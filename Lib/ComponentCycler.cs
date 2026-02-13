using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WINFORMS_VLCClient.Lib.ComponentCycler
{
    public class ComponentCycler
    {
        Control[] controls;
        int index;

        public ComponentCycler(Control[] controls)
        {
            this.controls = controls;
            index = 0;
        }

        public void ShowAtSlot(int slot)
        {
            if (slot > controls.Length)
                throw new Exception(
                    $"Requested Slot: {slot}, is greater than max allowed: {controls.Length - 1}"
                );

            foreach (Control control in controls)
                ToggleControl(control, false);

            ToggleControl(controls[slot], true);
            index = slot;
        }

        public void Cycle()
        {
            ToggleControl(controls[index], false);

            index += 1;
            if (index >= controls.Length)
                index = 0;

            ToggleControl(controls[index], true);
        }

        public int GetSlot() => index;

        void ToggleControl(Control control, bool state)
        {
            control.Visible = state;
            control.Enabled = state;
        }
    }
}
