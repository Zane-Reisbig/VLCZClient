using System.Diagnostics;
using System.IO.Pipes;
using ClientLib.STD;

namespace WINFORMS_VLCClient
{
    internal static class ZMediaEntry
    {
        const string ZMEDIA_MUTEX_ID = "cd424944-bc7d-48b3-817c-3b772ed26899";
        const string ZMEDIA_PIPE_SERVER_NAME = "690a2caf-beca-4770-9b60-572cc117872a";
        const string ZMEDIA_PIPE_MESSAGE_URL_PREFIX = "URL";
        const string ZMEDIA_PIPE_MESSAGE_EMPTY = "EMPTY";

        static bool createdMutex = false;
        static CancellationTokenSource? pipedMessageCancellationSource;
        static Landing? landingInst;
        static Mutex processLock = new(true, ZMEDIA_MUTEX_ID, out createdMutex);
        static NamedPipeServerStream pipeServer = null!;
        static NamedPipeClientStream pipeClient = null!;

        [STAThread]
        static void Main(string[] args)
        {
            string? passedArgs = null;
            if (args.Length >= 1 && File.Exists(args[0]))
                passedArgs = args[0];

            if (!createdMutex)
            {
                pipeClient = new NamedPipeClientStream(
                    ".",
                    ZMEDIA_PIPE_SERVER_NAME,
                    PipeDirection.Out,
                    PipeOptions.Asynchronous
                );

                ManualResetEventSlim pipedMessageEvent = new();
                Task.Run(async () =>
                {
                    await SendPipedMessage(passedArgs);
                    pipedMessageEvent.Set();
                });

                pipedMessageEvent.Wait();
                TearDown();
                return;
            }

            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();

            pipeServer = new NamedPipeServerStream(
                ZMEDIA_PIPE_SERVER_NAME,
                PipeDirection.In,
                5,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
            );

            landingInst = new Landing(passedArgs);
            pipedMessageCancellationSource = new();
            pipedMessageCancellationSource.Token.ThrowIfCancellationRequested();
            Task.Run(
                async () =>
                {
                    while (true)
                        await WaitForPipedMessage();
                },
                cancellationToken: pipedMessageCancellationSource.Token
            );

            Application.Run(landingInst);
            TearDown();
        }

        static async Task WaitForPipedMessage()
        {
            if (landingInst == null)
            {
                Debug.WriteLine(
                    $"WARN_ERROR: Server tried to connected before instance of Landing was created!"
                );
                return;
            }

            Debug.WriteLine("INFO: Server Created!");
            await pipeServer.WaitForConnectionAsync();

            Debug.WriteLine("INFO: Server recieved connection!");
            using (StreamReader sr = new(pipeServer, leaveOpen: true))
            {
                var prospectiveURL = await sr.ReadLineAsync();
                if (prospectiveURL == null)
                    return;

                var parts = prospectiveURL.Split("::");
                if (parts.Length == 0)
                {
                    Debug.WriteLine(
                        $"WARN: Malformed Pipe Message!\nOffender - \"{prospectiveURL}\""
                    );
                    return;
                }

                switch (parts[0])
                {
                    case ZMEDIA_PIPE_MESSAGE_URL_PREFIX:
                    {
                        if (!File.Exists(parts[1]))
                        {
                            Debug.WriteLine(
                                $"WARN: Requsted File does not exist!\nOffender - \"{parts[1]}\""
                            );
                            break;
                        }

                        StandardDefinitions.RunSafeInvoke(
                            landingInst,
                            () => landingInst.PlayMediaFromString(parts[1])
                        );
                        break;
                    }
                    default:
                        Debug.WriteLine($"WARN: Pipe-Prefix not found!\nOffender - \"{parts[0]}\"");
                        break;
                }
            }

            pipeServer.Disconnect();
        }

        static async Task SendPipedMessage(string? message)
        {
            if (!pipeClient.IsConnected)
                pipeClient.Connect();

            Debug.WriteLine("INFO: Client Connected!");

            var _message = message ?? ZMEDIA_PIPE_MESSAGE_EMPTY;
            using (StreamWriter sw = new(pipeClient))
            {
                sw.AutoFlush = true;
                await sw.WriteLineAsync($"{ZMEDIA_PIPE_MESSAGE_URL_PREFIX}::{_message}");
            }
            Debug.WriteLine($"INFO: Wrote Message: \"{_message}\"");
        }

        static bool UnlockProcess()
        {
            if (processLock == null)
                return false;

            processLock.ReleaseMutex();
            return true;
        }

        static void TearDown()
        {
            if (
                pipedMessageCancellationSource != null
                && !pipedMessageCancellationSource.IsCancellationRequested
            )
            {
                pipedMessageCancellationSource.Cancel();
                pipedMessageCancellationSource.Dispose();
            }

            if (createdMutex)
            {
                if (pipeServer.IsConnected)
                    pipeServer.Disconnect();

                pipeServer.Dispose();

                UnlockProcess();
            }
            else
                pipeClient.Dispose();
        }
    }
}
