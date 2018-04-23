using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace ReadOnlyDelayedProperty {
    class Program {
        static void Main(string[] args) {
            byte[] data = new byte[5] { 5, 4, 3, 2, 1 };

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema);
            //new Session().ClearDatabase();

            using(UnitOfWork uof = new UnitOfWork()) {
                if(uof.FindObject<MyObject>(null) == null) {
                    Console.WriteLine("Creating a default object...");
                    MyObject obj = new MyObject(uof);
                    obj.Name = "pict";
                    obj.DocumentInternal = data;
                    uof.CommitChanges();
                }
            }

            using(UnitOfWork uof = new UnitOfWork()) {
                Console.WriteLine("Loading MyObject...");
                MyObject obj = uof.FindObject<MyObject>(null);
                Console.WriteLine(obj.Name);
                byte[] loaded = obj.Document;
                Console.WriteLine(loaded.Length);
            }

            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }

    public class MyObject : XPObject {
        public MyObject(Session session) : base(session) { }

        string name;
        public string Name {
            get { return name; }
            set {
                if(IsLoading)
                    Console.WriteLine("Loading Name");
                SetPropertyValue("Name", ref name, value);
            }
        }

        [Delayed, Persistent("Document")]
        internal byte[] DocumentInternal {
            get {
                Console.WriteLine("Reading DocumentInternal");
                return GetDelayedPropertyValue<byte[]>("DocumentInternal"); 
            }
            set { SetDelayedPropertyValue("DocumentInternal", value); }
        }

        [PersistentAlias("DocumentInternal")]
        public byte[] Document {
            get { return DocumentInternal; }
        }

    }

}
