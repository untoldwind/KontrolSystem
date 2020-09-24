namespace KontrolSystem.TO2.Binding {
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class KSModule : System.Attribute {
        private string name;
        private string description;

        public KSModule(string name) => this.name = name;

        public string Name => name;

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface)]
    public class KSClass : System.Attribute {
        private string name;
        private string description;

        public KSClass(string name = null) => this.name = name;

        public string Name => name;

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class KSConstant : System.Attribute {
        private string name;
        private string description;

        public KSConstant(string name = null) => this.name = name;

        public string Name => name;

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class KSFunction : System.Attribute {
        private string name;
        private string description;

        public KSFunction(string name = null) => this.name = name;

        public string Name => name;

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class KSMethod : System.Attribute {
        private string name;
        private string description;

        public KSMethod(string name = null) => this.name = name;

        public string Name => name;

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public class KSField : System.Attribute {
        private string name;
        private string description;
        private bool includeSetter;

        public KSField(string name = null) => this.name = name;

        public string Name => name;

        public bool IncludeSetter {
            get => includeSetter;
            set => includeSetter = value;
        }

        public string Description {
            get => description;
            set => description = value;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class KSParameter : System.Attribute {
        private string description;

        public KSParameter(string description) => this.description = description;

        public string Description => description;
    }
}
