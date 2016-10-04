namespace System.Runtime.CompilerServices {
    internal class __BlockReflectionAttribute : Attribute { }
}

namespace Microsoft.Xml.Serialization.GeneratedAssembly {


    [System.Runtime.CompilerServices.__BlockReflection]
    public class XmlSerializationWriter1 : System.Xml.Serialization.XmlSerializationWriter {

        public void Write5_ArrayOfTransitStop(object o, string parentRuntimeNs = null, string parentCompileTimeNs = null) {
            string defaultNamespace = parentRuntimeNs ?? @"";
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ArrayOfTransitStop", defaultNamespace);
                return;
            }
            TopLevelElement();
            string namespace1 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
            {
                global::OneAppAway._1_1.Data.TransitStop[] a = (global::OneAppAway._1_1.Data.TransitStop[])((global::OneAppAway._1_1.Data.TransitStop[])o);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"ArrayOfTransitStop", defaultNamespace);
                }
                else {
                    WriteStartElement(@"ArrayOfTransitStop", namespace1, null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        string namespace2 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
                        Write4_TransitStop(@"TransitStop", namespace2, ((global::OneAppAway._1_1.Data.TransitStop)a[ia]), false, namespace2, @"");
                    }
                    WriteEndElement();
                }
            }
        }

        void Write4_TransitStop(string n, string ns, global::OneAppAway._1_1.Data.TransitStop o, bool needType, string parentRuntimeNs = null, string parentCompileTimeNs = null) {
            string defaultNamespace = parentRuntimeNs;
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::OneAppAway._1_1.Data.TransitStop)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TransitStop", defaultNamespace);
            WriteAttribute(@"ID", @"", ((global::System.String)o.@ID));
            WriteAttribute(@"Parent", @"", ((global::System.String)o.@Parent));
            WriteAttribute(@"Direction", @"", Write1_StopDirection(((global::OneAppAway._1_1.Data.StopDirection)o.@Direction)));
            WriteAttribute(@"Path", @"", ((global::System.String)o.@Path));
            WriteAttribute(@"Name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"Code", @"", ((global::System.String)o.@Code));
            WriteAttribute(@"Provider", @"", ((global::System.String)o.@Provider));
            WriteAttribute(@"ProviderID", @"", ((global::System.String)o.@ProviderID));
            WriteAttribute(@"Status", @"", Write2_AlertStatus(((global::OneAppAway._1_1.Data.AlertStatus)o.@Status)));
            string namespace3 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
            WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::OneAppAway._1_1.Data.LatLon)o.@Position), @"Position", namespace3, false, true);
            string namespace4 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
            {
                global::System.String[] a = (global::System.String[])((global::System.String[])o.@Children);
                if (a != null){
                    WriteStartElement(@"Children", namespace4, null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        string namespace5 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
                        WriteNullableStringLiteral(@"string", namespace5, ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            string namespace6 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
            {
                global::System.String[] a = (global::System.String[])((global::System.String[])o.@Routes);
                if (a != null){
                    WriteStartElement(@"Routes", namespace6, null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        string namespace7 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
                        WriteNullableStringLiteral(@"string", namespace7, ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            string namespace8 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
            {
                global::System.String[] a = (global::System.String[])((global::System.String[])o.@Alerts);
                if (a != null){
                    WriteStartElement(@"Alerts", namespace8, null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        string namespace9 = ( parentCompileTimeNs == @"" && parentRuntimeNs != null ) ? parentRuntimeNs : @"";
                        WriteNullableStringLiteral(@"string", namespace9, ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        string Write2_AlertStatus(global::OneAppAway._1_1.Data.AlertStatus v) {
            string s = null;
            switch (v) {
                case global::OneAppAway._1_1.Data.AlertStatus.@Normal: s = @"Normal"; break;
                case global::OneAppAway._1_1.Data.AlertStatus.@Alert: s = @"Alert"; break;
                case global::OneAppAway._1_1.Data.AlertStatus.@Cancelled: s = @"Cancelled"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"OneAppAway._1_1.Data.AlertStatus");
            }
            return s;
        }

        string Write1_StopDirection(global::OneAppAway._1_1.Data.StopDirection v) {
            string s = null;
            switch (v) {
                case global::OneAppAway._1_1.Data.StopDirection.@Unspecified: s = @"Unspecified"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@N: s = @"N"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@NE: s = @"NE"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@E: s = @"E"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@SE: s = @"SE"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@S: s = @"S"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@SW: s = @"SW"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@W: s = @"W"; break;
                case global::OneAppAway._1_1.Data.StopDirection.@NW: s = @"NW"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"OneAppAway._1_1.Data.StopDirection");
            }
            return s;
        }

        protected override void InitCallbacks() {
        }
    }

    [System.Runtime.CompilerServices.__BlockReflection]
    public class XmlSerializationReader1 : System.Xml.Serialization.XmlSerializationReader {

        public object Read5_ArrayOfTransitStop(string defaultNamespace = null) {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_ArrayOfTransitStop && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                    if (!ReadNull()) {
                        global::OneAppAway._1_1.Data.TransitStop[] a_0_0 = null;
                        int ca_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations0 = 0;
                            int readerCount0 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id3_TransitStop && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                                        a_0_0 = (global::OneAppAway._1_1.Data.TransitStop[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::OneAppAway._1_1.Data.TransitStop));a_0_0[ca_0_0++] = Read4_TransitStop(true, defaultNamespace);
                                    }
                                    else {
                                        UnknownNode(null, @":TransitStop");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":TransitStop");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations0, ref readerCount0);
                            }
                        ReadEndElement();
                        }
                        o = (global::OneAppAway._1_1.Data.TransitStop[])ShrinkArray(a_0_0, ca_0_0, typeof(global::OneAppAway._1_1.Data.TransitStop), false);
                    }
                    else {
                        global::OneAppAway._1_1.Data.TransitStop[] a_0_0 = null;
                        int ca_0_0 = 0;
                        o = (global::OneAppAway._1_1.Data.TransitStop[])ShrinkArray(a_0_0, ca_0_0, typeof(global::OneAppAway._1_1.Data.TransitStop), true);
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, defaultNamespace ?? @":ArrayOfTransitStop");
            }
            return (object)o;
        }

        global::OneAppAway._1_1.Data.TransitStop Read4_TransitStop(bool checkType, string defaultNamespace = null) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id3_TransitStop && string.Equals( ((System.Xml.XmlQualifiedName)xsiType).Namespace, defaultNamespace ?? id2_Item))) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            global::OneAppAway._1_1.Data.TransitStop o;
            try {
                o = (global::OneAppAway._1_1.Data.TransitStop)ActivatorHelper.CreateInstance(typeof(global::OneAppAway._1_1.Data.TransitStop));
            }
            catch (System.MissingMemberException) {
                throw CreateInaccessibleConstructorException(@"global::OneAppAway._1_1.Data.TransitStop");
            }
            catch (System.Security.SecurityException) {
                throw CreateCtorHasSecurityException(@"global::OneAppAway._1_1.Data.TransitStop");
            }
            global::System.String[] a_9 = null;
            int ca_9 = 0;
            global::System.String[] a_10 = null;
            int ca_10 = 0;
            global::System.String[] a_11 = null;
            int ca_11 = 0;
            bool[] paramsRead = new bool[13];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id4_ID && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@ID = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id5_Parent && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Parent = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id6_Direction && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Direction = Read1_StopDirection(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id7_Path && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Path = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id8_Name && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Name = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object) Reader.LocalName == (object)id9_Code && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Code = Reader.Value;
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object) Reader.LocalName == (object)id10_Provider && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Provider = Reader.Value;
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object) Reader.LocalName == (object)id11_ProviderID && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@ProviderID = Reader.Value;
                    paramsRead[8] = true;
                }
                else if (!paramsRead[12] && ((object) Reader.LocalName == (object)id12_Status && string.Equals(Reader.NamespaceURI, id2_Item))) {
                    o.@Status = Read2_AlertStatus(Reader.Value);
                    paramsRead[12] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o, @":ID, :Parent, :Direction, :Path, :Name, :Code, :Provider, :ProviderID, :Status");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations1 = 0;
            int readerCount1 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (!paramsRead[2] && ((object) Reader.LocalName == (object)id13_Position && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                        o.@Position = (global::OneAppAway._1_1.Data.LatLon)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)ActivatorHelper.CreateInstance(typeof(global::OneAppAway._1_1.Data.LatLon)));
                        paramsRead[2] = true;
                    }
                    else if (((object) Reader.LocalName == (object)id14_Children && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                        if (!ReadNull()) {
                            global::System.String[] a_9_0 = null;
                            int ca_9_0 = 0;
                            if ((Reader.IsEmptyElement)) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                int whileIterations2 = 0;
                                int readerCount2 = ReaderCount;
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id15_string && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                                            if (ReadNull()) {
                                                a_9_0 = (global::System.String[])EnsureArrayIndex(a_9_0, ca_9_0, typeof(global::System.String));a_9_0[ca_9_0++] = null;
                                            }
                                            else {
                                                a_9_0 = (global::System.String[])EnsureArrayIndex(a_9_0, ca_9_0, typeof(global::System.String));a_9_0[ca_9_0++] = Reader.ReadElementContentAsString();
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":string");
                                    }
                                    Reader.MoveToContent();
                                    CheckReaderCount(ref whileIterations2, ref readerCount2);
                                }
                            ReadEndElement();
                            }
                            o.@Children = (global::System.String[])ShrinkArray(a_9_0, ca_9_0, typeof(global::System.String), false);
                        }
                    }
                    else if (((object) Reader.LocalName == (object)id16_Routes && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                        if (!ReadNull()) {
                            global::System.String[] a_10_0 = null;
                            int ca_10_0 = 0;
                            if ((Reader.IsEmptyElement)) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                int whileIterations3 = 0;
                                int readerCount3 = ReaderCount;
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id15_string && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                                            if (ReadNull()) {
                                                a_10_0 = (global::System.String[])EnsureArrayIndex(a_10_0, ca_10_0, typeof(global::System.String));a_10_0[ca_10_0++] = null;
                                            }
                                            else {
                                                a_10_0 = (global::System.String[])EnsureArrayIndex(a_10_0, ca_10_0, typeof(global::System.String));a_10_0[ca_10_0++] = Reader.ReadElementContentAsString();
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":string");
                                    }
                                    Reader.MoveToContent();
                                    CheckReaderCount(ref whileIterations3, ref readerCount3);
                                }
                            ReadEndElement();
                            }
                            o.@Routes = (global::System.String[])ShrinkArray(a_10_0, ca_10_0, typeof(global::System.String), false);
                        }
                    }
                    else if (((object) Reader.LocalName == (object)id17_Alerts && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                        if (!ReadNull()) {
                            global::System.String[] a_11_0 = null;
                            int ca_11_0 = 0;
                            if ((Reader.IsEmptyElement)) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                int whileIterations4 = 0;
                                int readerCount4 = ReaderCount;
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id15_string && string.Equals(Reader.NamespaceURI, defaultNamespace ?? id2_Item))) {
                                            if (ReadNull()) {
                                                a_11_0 = (global::System.String[])EnsureArrayIndex(a_11_0, ca_11_0, typeof(global::System.String));a_11_0[ca_11_0++] = null;
                                            }
                                            else {
                                                a_11_0 = (global::System.String[])EnsureArrayIndex(a_11_0, ca_11_0, typeof(global::System.String));a_11_0[ca_11_0++] = Reader.ReadElementContentAsString();
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":string");
                                    }
                                    Reader.MoveToContent();
                                    CheckReaderCount(ref whileIterations4, ref readerCount4);
                                }
                            ReadEndElement();
                            }
                            o.@Alerts = (global::System.String[])ShrinkArray(a_11_0, ca_11_0, typeof(global::System.String), false);
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Position, :Children, :Routes, :Alerts");
                    }
                }
                else {
                    UnknownNode((object)o, @":Position, :Children, :Routes, :Alerts");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations1, ref readerCount1);
            }
            ReadEndElement();
            return o;
        }

        global::OneAppAway._1_1.Data.AlertStatus Read2_AlertStatus(string s) {
            switch (s) {
                case @"Normal": return global::OneAppAway._1_1.Data.AlertStatus.@Normal;
                case @"Alert": return global::OneAppAway._1_1.Data.AlertStatus.@Alert;
                case @"Cancelled": return global::OneAppAway._1_1.Data.AlertStatus.@Cancelled;
                default: throw CreateUnknownConstantException(s, typeof(global::OneAppAway._1_1.Data.AlertStatus));
            }
        }

        global::OneAppAway._1_1.Data.StopDirection Read1_StopDirection(string s) {
            switch (s) {
                case @"Unspecified": return global::OneAppAway._1_1.Data.StopDirection.@Unspecified;
                case @"N": return global::OneAppAway._1_1.Data.StopDirection.@N;
                case @"NE": return global::OneAppAway._1_1.Data.StopDirection.@NE;
                case @"E": return global::OneAppAway._1_1.Data.StopDirection.@E;
                case @"SE": return global::OneAppAway._1_1.Data.StopDirection.@SE;
                case @"S": return global::OneAppAway._1_1.Data.StopDirection.@S;
                case @"SW": return global::OneAppAway._1_1.Data.StopDirection.@SW;
                case @"W": return global::OneAppAway._1_1.Data.StopDirection.@W;
                case @"NW": return global::OneAppAway._1_1.Data.StopDirection.@NW;
                default: throw CreateUnknownConstantException(s, typeof(global::OneAppAway._1_1.Data.StopDirection));
            }
        }

        protected override void InitCallbacks() {
        }

        string id1_ArrayOfTransitStop;
        string id4_ID;
        string id7_Path;
        string id13_Position;
        string id15_string;
        string id16_Routes;
        string id8_Name;
        string id9_Code;
        string id10_Provider;
        string id3_TransitStop;
        string id11_ProviderID;
        string id5_Parent;
        string id12_Status;
        string id17_Alerts;
        string id2_Item;
        string id6_Direction;
        string id14_Children;

        protected override void InitIDs() {
            id1_ArrayOfTransitStop = Reader.NameTable.Add(@"ArrayOfTransitStop");
            id4_ID = Reader.NameTable.Add(@"ID");
            id7_Path = Reader.NameTable.Add(@"Path");
            id13_Position = Reader.NameTable.Add(@"Position");
            id15_string = Reader.NameTable.Add(@"string");
            id16_Routes = Reader.NameTable.Add(@"Routes");
            id8_Name = Reader.NameTable.Add(@"Name");
            id9_Code = Reader.NameTable.Add(@"Code");
            id10_Provider = Reader.NameTable.Add(@"Provider");
            id3_TransitStop = Reader.NameTable.Add(@"TransitStop");
            id11_ProviderID = Reader.NameTable.Add(@"ProviderID");
            id5_Parent = Reader.NameTable.Add(@"Parent");
            id12_Status = Reader.NameTable.Add(@"Status");
            id17_Alerts = Reader.NameTable.Add(@"Alerts");
            id2_Item = Reader.NameTable.Add(@"");
            id6_Direction = Reader.NameTable.Add(@"Direction");
            id14_Children = Reader.NameTable.Add(@"Children");
        }
    }

    [System.Runtime.CompilerServices.__BlockReflection]
    public abstract class XmlSerializer1 : System.Xml.Serialization.XmlSerializer {
        protected override System.Xml.Serialization.XmlSerializationReader CreateReader() {
            return new XmlSerializationReader1();
        }
        protected override System.Xml.Serialization.XmlSerializationWriter CreateWriter() {
            return new XmlSerializationWriter1();
        }
    }

    [System.Runtime.CompilerServices.__BlockReflection]
    public sealed class ArrayOfTransitStopSerializer : XmlSerializer1 {

        public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
            return xmlReader.IsStartElement(@"ArrayOfTransitStop", this.DefaultNamespace ?? @"");
        }

        protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
            ((XmlSerializationWriter1)writer).Write5_ArrayOfTransitStop(objectToSerialize, this.DefaultNamespace, @"");
        }

        protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
            return ((XmlSerializationReader1)reader).Read5_ArrayOfTransitStop(this.DefaultNamespace);
        }
    }

    [System.Runtime.CompilerServices.__BlockReflection]
    public class XmlSerializerContract : global::System.Xml.Serialization.XmlSerializerImplementation {
        public override global::System.Xml.Serialization.XmlSerializationReader Reader { get { return new XmlSerializationReader1(); } }
        public override global::System.Xml.Serialization.XmlSerializationWriter Writer { get { return new XmlSerializationWriter1(); } }
        System.Collections.IDictionary readMethods = null;
        public override System.Collections.IDictionary ReadMethods {
            get {
                if (readMethods == null) {
                    System.Collections.IDictionary _tmp = new System.Collections.Generic.Dictionary<string, string>();
                    _tmp[@"OneAppAway._1_1.Data.TransitStop[]::"] = @"Read5_ArrayOfTransitStop";
                    if (readMethods == null) readMethods = _tmp;
                }
                return readMethods;
            }
        }
        System.Collections.IDictionary writeMethods = null;
        public override System.Collections.IDictionary WriteMethods {
            get {
                if (writeMethods == null) {
                    System.Collections.IDictionary _tmp = new System.Collections.Generic.Dictionary<string, string>();
                    _tmp[@"OneAppAway._1_1.Data.TransitStop[]::"] = @"Write5_ArrayOfTransitStop";
                    if (writeMethods == null) writeMethods = _tmp;
                }
                return writeMethods;
            }
        }
        System.Collections.IDictionary typedSerializers = null;
        public override System.Collections.IDictionary TypedSerializers {
            get {
                if (typedSerializers == null) {
                    System.Collections.IDictionary _tmp = new System.Collections.Generic.Dictionary<string, System.Xml.Serialization.XmlSerializer>();
                    _tmp.Add(@"OneAppAway._1_1.Data.TransitStop[]::", new ArrayOfTransitStopSerializer());
                    if (typedSerializers == null) typedSerializers = _tmp;
                }
                return typedSerializers;
            }
        }
        public override System.Boolean CanSerialize(System.Type type) {
            if (type == typeof(global::OneAppAway._1_1.Data.TransitStop[])) return true;
            if (type == typeof(global::System.Reflection.TypeInfo)) return true;
            return false;
        }
        public override System.Xml.Serialization.XmlSerializer GetSerializer(System.Type type) {
            if (type == typeof(global::OneAppAway._1_1.Data.TransitStop[])) return new ArrayOfTransitStopSerializer();
            return null;
        }
        public static global::System.Xml.Serialization.XmlSerializerImplementation GetXmlSerializerContract() { return new XmlSerializerContract(); }
    }

    [System.Runtime.CompilerServices.__BlockReflection]
    public static class ActivatorHelper {
        public static object CreateInstance(System.Type type) {
            System.Reflection.TypeInfo ti = System.Reflection.IntrospectionExtensions.GetTypeInfo(type);
            foreach (System.Reflection.ConstructorInfo ci in ti.DeclaredConstructors) {
                if (!ci.IsStatic && ci.GetParameters().Length == 0) {
                    return ci.Invoke(null);
                }
            }
            return System.Activator.CreateInstance(type);
        }
    }
}
