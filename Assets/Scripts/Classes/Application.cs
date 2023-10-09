using System.Collections.Generic;

namespace ReceiveData {
    
    [System.Serializable]
    public class ConnectedObject {
        public string objname;
        public string objtype;
        public string hstate;
        public string reada;
        public List<string> openopts;
    }

    [System.Serializable]
    public class Application {
        public string conn;
        public string channel;
        public string type;
        public string conntag;
        public string conname;
        public List<string> connopts;
        public string appltype;
        public string appldesc;
        public string appltag;
        public List<ConnectedObject> connected_objects;
    }
}
