using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace LiveSplit.OriAndTheBlindForest
{
    public class OriMemory
    {
        public string ingame_pattern = "8D65FC5FC9C300000000558BEC83EC08B9????????8B4508890183EC0C50E8????????83C410C9C3|-23";
        public string events_pattern = "85C0740AB850000000E97A0000000FB605????????85C0740AB83C000000E9650000000FB605????????85C0740AB832000000E9500000000FB605????????85C07407B828000000EB3E|3";
        public string position_pattern = "05480000008B08894DE88B4804894DEC8B40088945F08B05";

        public int pPosition = 0;
        public int pEvents = 0;
        public int pInGame = 0;

        public Process proc;
        public bool isHooked = false;
        public bool isExited = false;

        public DateTime hookTime;

        public bool HookProcess()
        {
            if (proc == null || proc.HasExited)
            {
                // Reset Pointers
                pEvents = 0;
                pPosition = 0;
                pInGame = 0;

                Process[] processes = Process.GetProcessesByName("ori");

                if (processes.Length == 0)
                {
                    isHooked = false;
                    return isHooked;
                }

                proc = processes[0];

                if (proc.HasExited)
                {
                    isHooked = false;
                    return isHooked;
                }

                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(ProcessExited);

                isHooked = true;
                isExited = false;
                hookTime = DateTime.Now;
            }

            return isHooked;
        }

        public void ProcessExited(object sender, System.EventArgs e)
        {
            isExited = true;
        }

        /*
        public void PrintPosition()
        {
            foreach (var pair in OriState.axes)
            {
                Console.WriteLine("{0}: {1}",
                    pair.Key,
                    GetPosition(pair.Value).ToString());
            }
        }

        public void PrintInGame()
        {
            Console.WriteLine("In Game? " + (GetInGame()).ToString());
        }

        public void PrintEvents()
        {
            foreach (var pair in OriState.events)
            {
                Console.WriteLine("{0}: {1}",
                    pair.Key,
                    GetEvent(pair.Value).ToString());
            }

            foreach (var pair in OriState.keys)
            {
                Console.WriteLine("{0}: {1}",
                    pair.Key,
                    GetKey(pair.Value).ToString());
            }
        }

        public void PrintAbilities(Process proc, int pAbilities)
        {
            foreach (var pair in OriState.abilities)
            {
                Console.WriteLine("{0}: {1}",
                    pair.Key,
                    GetAbility(proc, pAbilities, pair.Value).ToString());
            }
        }
        */

        public float GetPosition(int axis_id)
        {
            if (pPosition == 0)
            {
                int[] positionaddrs = Memory.FindMemorySignatures(proc,
                    position_pattern
                );
                pPosition = positionaddrs[0];
                pPosition = pPosition - 0x1C;
            }

            // Resolved mono + 0x001F42C4 [0x50, 0x688, 0x7F0, 0x478, 0x750]
            // Found assembly that accessed it.

            int start = Memory.GetInt32(proc, pPosition);
            int path0 = Memory.GetInt32(proc, start);
            return Memory.GetFloat(proc, path0 + 0x48 + (axis_id * 4));
        }

        public bool GetInGame()
        {
            if (pInGame == 0)
            {
                int[] ingameaddrs = Memory.FindMemorySignatures(proc,
                    ingame_pattern
                );

                pInGame = ingameaddrs[0];
            }

            int start = Memory.GetInt32(proc, pInGame);
            int path0 = Memory.GetInt32(proc, start);
            return !Memory.GetBool(proc, path0 + 0x69);
        }

        public bool GetEvent(int event_id)
        {
            if (pEvents == 0)
            {
                int[] eventsaddrs = Memory.FindMemorySignatures(proc,
                    events_pattern
                );
                pEvents = eventsaddrs[0];
            }

            int start = Memory.GetInt32(proc, pEvents);
            return Memory.GetBool(proc, start + event_id);
        }

        public bool GetKey(int event_id)
        {
            if (pEvents == 0)
            {
                int[] eventsaddrs = Memory.FindMemorySignatures(proc,
                    events_pattern
                );
                pEvents = eventsaddrs[0];
            }

            int start = Memory.GetInt32(proc, pEvents);
            return Memory.GetBool(proc, start - 0x40 + event_id);
        }

        /*
        public bool GetAbility(int ability_id)
        {
            if (pAbilities != 0)
            {
                int start = Memory.GetInt32(proc, pAbilities);
                int path0 = Memory.GetInt32(proc, start);
                int path1 = Memory.GetInt32(proc, path0 + 0x4C);
                int path2 = Memory.GetInt32(proc, path1 + (ability_id * 4));
                return Memory.GetBool(proc, path2 + 0x8);
            }
            return false;
        }
        */
    }
}