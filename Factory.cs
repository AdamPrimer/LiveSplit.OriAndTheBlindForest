using System.Reflection;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

namespace LiveSplit.OriAndTheBlindForest
{
    public class Factory : IComponentFactory
    {
        public string ComponentName {
            get { return "Ori and the Blind Forest Autosplitter"; }
        }

        public string Description {
            get { return "Autosplitter for Ori and the Blind Forest"; }
        }

        public ComponentCategory Category {
            get { return ComponentCategory.Control; }
        }

        public IComponent Create(LiveSplitState state) {
            return new OriComponent();
        }

        public string UpdateName {
            get { return ""; }
        }

        public string UpdateURL {
            get { return "http://livesplit.org/update/"; }
        }

        public Version Version {
            get { return new Version(); }
        }

        public string XMLURL {
            get { return "http://livesplit.org/update/Components/noupdates.xml"; }
        }
    }
}
