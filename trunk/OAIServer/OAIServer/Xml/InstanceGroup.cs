namespace OAIServer.Xml {
    using System.Xml;
    using System.Xml.Schema;
    using System.Collections;
    
    internal class InstanceGroup : InstanceObject {
        decimal maxOccurs = 1;
        decimal minOccurs = 0;
        bool isChoice = false;

        InstanceGroup sibling;
        InstanceGroup child;

        internal InstanceGroup() {
        }
        
        internal decimal MaxOccurs {
            get {
                return maxOccurs;
            }

            set {
                maxOccurs = value;
            }
        }

        internal decimal MinOccurs
        {
            get
            {
                return minOccurs;
            }

            set
            {
                minOccurs = value;
            }
        }
        
        internal bool IsChoice {
            get {
                return isChoice;
            }
            set {
                isChoice = value;
            }
        }

        internal InstanceGroup Sibling {
            get {
                return sibling;
            }
            set {
                sibling = value;
            }
        }
        
        internal InstanceGroup Child {
            get {
                return child;
            }
            set {
                child = value;
            }
        }
        internal int NoOfChildren {
            get {
                int cnt = 0;
                InstanceGroup currentGroup = this.child;
                while (currentGroup != null) {
                    cnt++;
                    currentGroup = currentGroup.Sibling;
                }
                return cnt;
            }
        }

        internal void AddChild (InstanceGroup obj) {
            obj.Parent = this;
            if (this.child == null) { //If first child
                this.child = obj;
            }
            else {
                InstanceGroup prev = null;
                InstanceGroup next = this.child;
                while (next != null) {
                    prev = next;
                    next = next.Sibling;
                }
                prev.Sibling = obj;
            }
        }

        internal InstanceGroup GetChild(int index) {
            int curIndex = 0;
            InstanceGroup currentGroup = this.child;
            while (currentGroup != null) {
                if (curIndex == index) {
                    return currentGroup;
                }
                curIndex++;
                currentGroup = currentGroup.Sibling;
            }
            return null;
        }
    }
}

