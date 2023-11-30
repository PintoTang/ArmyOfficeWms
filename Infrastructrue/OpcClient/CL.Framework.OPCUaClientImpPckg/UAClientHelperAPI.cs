using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;


namespace CL.Framework.OPCUaClientImpPckg
{
    public class UAClientHelperAPI
    {
        #region Construction
        public UAClientHelperAPI()
        {
            // Creats the application configuration (containing the certificate) on construction
            _mApplicationConfig = CreateClientConfiguration();
        }
        #endregion

        #region Properties
        /// <summary> 
        /// Keeps a session with an UA server. 
        /// </summary>
        private Session _mSession = null;

        /// <summary> 
        /// Specifies this application 
        /// </summary>
        private ApplicationConfiguration _mApplicationConfig = null;

        /// <summary>
        /// Provides the session being established with an OPC UA server.
        /// </summary>
        public Session Session
        {
            get { return _mSession; }
        }

        /// <summary>
        /// Provides the event for value changes of a monitored item.
        /// </summary>
        public event MonitoredItemNotificationEventHandler ItemChangedNotification = null;

        /// <summary>
        /// Provides the event for KeepAliveNotifications.
        /// </summary>
        public event KeepAliveEventHandler KeepAliveNotification = null;
        #endregion

        #region Discovery
        /// <summary>Finds Servers based on a discovery url</summary>
        /// <param name="discoveryUrl">The discovery url</param>
        /// <returns>ApplicationDescriptionCollection containing found servers</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ApplicationDescriptionCollection FindServers(string discoveryUrl)
        {
            //Create a URI using the discovery URL
            Uri uri = new Uri(discoveryUrl);
            try
            {
                //Ceate a DiscoveryClient
                DiscoveryClient client = DiscoveryClient.Create(uri);
                //Find servers
                ApplicationDescriptionCollection servers = client.FindServers(null);
                return servers;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Finds Endpoints based on a server's url</summary>
        /// <param name="discoveryUrl">The server's url</param>
        /// <returns>EndpointDescriptionCollection containing found Endpoints</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public EndpointDescriptionCollection GetEndpoints(string serverUrl)
        {
            //Create a URI using the server's URL
            Uri uri = new Uri(serverUrl);
            try
            {
                //Create a DiscoveryClient
                DiscoveryClient client = DiscoveryClient.Create(uri);
                //Search for available endpoints
                EndpointDescriptionCollection endpoints = client.GetEndpoints(null);
                return endpoints;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }
        #endregion

        #region Connect/Disconnect
        /// <summary>Establishes the connection to an OPC UA server and creates a session using a server url.</summary>
        /// <param name="url">The Url of the endpoint as string.</param>
        /// <param name="localIpAddress">The ip address of the interface to connect with</param>
        /// <param name="security">Use security or not</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public async Task Connect(string url, bool security)
        {
            try
            {
                //Secify application configuration
                ApplicationConfiguration ApplicationConfig = _mApplicationConfig;

                //Hook up a validator function for a CertificateValidation event
                _mApplicationConfig.CertificateValidator.CertificateValidation += Validator_CertificateValidation;

                //Create EndPoint description
                EndpointDescription EndpointDescription = CreateEndpointDescription(url, security);

                //Create EndPoint configuration
                EndpointConfiguration EndpointConfiguration = EndpointConfiguration.Create(ApplicationConfig);

                //Create an Endpoint object to connect to server
                ConfiguredEndpoint Endpoint = new ConfiguredEndpoint(null, EndpointDescription, EndpointConfiguration);

                //Create anonymous user identity
                UserIdentity UserIdentity = new UserIdentity();

                //Create and connect session
                _mSession = await Session.Create(
                    ApplicationConfig,
                    Endpoint,
                    true,
                    "MySession",
                    60000,
                    UserIdentity,
                    null
                    );

                _mSession.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);

            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Establishes the connection to an OPC UA server and creates a session using a EndpointDescription.</summary>
        /// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
        /// <param name="localIpAddress">The ip address of the interface to connect with</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public async Task Connect(EndpointDescription endpointDescription)
        {
            try
            {
                //Secify application configuration
                ApplicationConfiguration ApplicationConfig = _mApplicationConfig;

                //Hook up a validator function for a CertificateValidation event
                ApplicationConfig.CertificateValidator.CertificateValidation += Validator_CertificateValidation;

                //Create EndPoint configuration
                EndpointConfiguration EndpointConfiguration = EndpointConfiguration.Create(ApplicationConfig);

                //Connect to server and get endpoints
                ConfiguredEndpoint mEndpoint = new ConfiguredEndpoint(null, endpointDescription, EndpointConfiguration);

                //Create the binding factory.
                //BindingFactory bindingFactory = BindingFactory.Create(mApplicationConfig, ServiceMessageContext.GlobalContext);

                //Create anonymous user identity
                UserIdentity UserIdentity = new UserIdentity();

                //Create and connect session
                _mSession = await Session.Create(
                    ApplicationConfig,
                    mEndpoint,
                    true,
                    "MySession",
                    60000,
                    UserIdentity,
                    null
                    );

                _mSession.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Closes an existing session and disconnects from the server.</summary>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Disconnect()
        {
            // Close the session.
            try
            {
                if (_mSession != null)
                {
                    _mSession.Close(10000);
                    _mSession.Dispose();
                }
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }
        #endregion

        #region Browse
        /// <summary>Browses the root folder of an OPC UA server.</summary>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseRoot()
        {
            //Create a collection for the browse results
            ReferenceDescriptionCollection referenceDescriptionCollection;
            //Create a continuationPoint
            byte[] continuationPoint;
            try
            {
                //Browse the RootFolder for variables, objects and methods
                _mSession.Browse(null, null, ObjectIds.RootFolder, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out continuationPoint, out referenceDescriptionCollection);
                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }

        }

        /// <summary>Browses a node ID provided by a ReferenceDescription</summary>
        /// <param name="refDesc">The ReferenceDescription</param>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseNode(ReferenceDescription refDesc)
        {
            //Create a collection for the browse results
            ReferenceDescriptionCollection referenceDescriptionCollection;
            //Create a continuationPoint
            byte[] continuationPoint;
            //Create a NodeId using the selected ReferenceDescription as browsing starting point
            NodeId nodeId = ExpandedNodeId.ToNodeId(refDesc.NodeId, null);
            try
            {
                //Browse from starting point for all object types
                _mSession.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out continuationPoint, out referenceDescriptionCollection);
                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }

        }
        #endregion

        #region Subscription
        /// <summary>Creats a Subscription object to a server</summary>
        /// <param name="publishingInterval">The publishing interval</param>
        /// <returns>Subscription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Subscription Subscribe(int publishingInterval)
        {
            //Create a Subscription object
            Subscription subscription = new Subscription(_mSession.DefaultSubscription);
            //Enable publishing
            subscription.PublishingEnabled = true;
            //Set the publishing interval
            subscription.PublishingInterval = publishingInterval;
            //Add the subscription to the session
            _mSession.AddSubscription(subscription);
            try
            {
                //Create/Activate the subscription
                subscription.Create();
                return subscription;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Ads a monitored item to an existing subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <param name="nodeIdString">The node Id as string</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public MonitoredItem AddMonitoredItem(Subscription subscription, string nodeIdString, int samplingInterval)
        {
            //Create a monitored item    
            MonitoredItem monitoredItem = new MonitoredItem(subscription.DefaultItem);
            //Set the NodeId of the item
            monitoredItem.StartNodeId = nodeIdString;
            //Set the attribute Id (value here)
            monitoredItem.AttributeId = Attributes.Value;
            //Set reporting mode 
            monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            //Set the sampling interval (1 = fastest possible)
            monitoredItem.SamplingInterval = samplingInterval;
            //Set the queue size
            monitoredItem.QueueSize = 0;
            //Discard the oldest item after new one has been received
            monitoredItem.DiscardOldest = true;
            //Define event handler for this item and then add to monitoredItem
            monitoredItem.Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
            try
            {
                //Add the item to the subscription
                subscription.AddItem(monitoredItem);
                //Apply changes to the subscription
                subscription.ApplyChanges();
                return monitoredItem;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Removs a monitored item from an existing subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <param name="monitoredItem">The item</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public MonitoredItem RemoveMonitoredItem(Subscription subscription, MonitoredItem monitoredItem)
        {
            try
            {
                //Add the item to the subscription
                subscription.RemoveItem(monitoredItem);
                //Apply changes to the subscription
                subscription.ApplyChanges();
                return null;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Removes an existing Subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void RemoveSubscription(Subscription subscription)
        {
            try
            {
                //Delete the subscription and all items submitted
                subscription.Delete(true);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }
        #endregion

        #region Read/Write
        /// <summary>Reads a node by node Id</summary>
        /// <param name="nodeIdString">The node Id as string</param>
        /// <returns>The read node</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Node ReadNode(string nodeIdString)
        {
            //Create a nodeId using the identifier string
            NodeId nodeId = new NodeId(nodeIdString);
            //Create a node
            Node node = new Node();
            try
            {
                //Read the dataValue
                node = _mSession.ReadNode(nodeId);
                return node;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="nodeIdAddr"></param>
        /// <returns></returns>
        public string WriteValue<T>(T value,string nodeIdAddr)
        {
            try
            {
                WriteValueCollection valuesToWrite = new WriteValueCollection();
                NodeId nodeId = new NodeId(nodeIdAddr);
                Variant variant = new Variant(value);
                DataValue dataValue = new DataValue(variant);
                WriteValue valueToWrite = new WriteValue();
                valueToWrite.Value = dataValue;
                valueToWrite.NodeId = nodeId;
                valueToWrite.AttributeId = Attributes.Value;

                valuesToWrite.Add(valueToWrite);
                //Write the collection to the server
                _mSession.Write(null, valuesToWrite, out StatusCodeCollection result, out DiagnosticInfoCollection diagnostics);
                string returnVal = result[0].ToString();
                return returnVal;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;

            }
        }

        /// <summary>Writes values to  node Ids</summary>
        /// <param name="value">The values as strings</param>
        /// <param name="nodeIdString">The nodes Id as strings</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public string WriteValues(List<string> values, List<string> nodeIdStrings)
        {
            //Create a collection of values to write
            WriteValueCollection valuesToWrite = new WriteValueCollection();
            //Create a collection for StatusCodes
            StatusCodeCollection result = new StatusCodeCollection();
            //Create a collection for DiagnosticInfos
            DiagnosticInfoCollection diagnostics = new DiagnosticInfoCollection();
            string returnVal = "";
            foreach (string str in nodeIdStrings)
            {
                //Create a nodeId
                NodeId nodeId = new NodeId(str);
                //Create a dataValue
                DataValue dataValue = new DataValue();
                //Read the dataValue
                try
                {
                    dataValue = _mSession.ReadValue(nodeId);
                }
                catch (Exception e)
                {
                    //handle Exception here
                    throw e;
                }
                //Get the data type of the read dataValue
                string dataType = dataValue.Value.GetType().FullName;
                dynamic setDataValues;
                if (dataType == "System.Byte[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    Byte[] tempStr = new Byte[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        Byte i = Convert.ToByte(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.Boolean[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    Boolean[] tempStr = new Boolean[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        Boolean i = Convert.ToBoolean(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.Int16[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    Int16[] tempStr = new Int16[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        Int16 i = Convert.ToInt16(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.UInt16[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    UInt16[] tempStr = new UInt16[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        UInt16 i = Convert.ToUInt16(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.UInt32[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    UInt32[] tempStr = new UInt32[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        UInt32 i = Convert.ToUInt32(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.Int64[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    Int64[] tempStr = new Int64[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        Int64 i = Convert.ToInt64(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else if (dataType == "System.Single[]")
                {
                    string[] tempValues = values[nodeIdStrings.IndexOf(str)].ToString().Split(';');
                    Single[] tempStr = new Single[tempValues.Length];
                    for (int j = 0; j < tempValues.Length; j++)
                    {
                        Single i = Convert.ToSingle(tempValues[j]);
                        tempStr[j] = i;
                    }
                    setDataValues = tempStr;
                }
                else
                {
                    setDataValues = values[nodeIdStrings.IndexOf(str)].ToString();
                }
                Variant variant = new Variant(Convert.ChangeType(setDataValues, dataValue.Value.GetType()));
                // Variant variant = new Variant(Convert.ChangeType(values[nodeIdStrings.IndexOf(str)], dataValue.Value.GetType()));
                //Overwrite the dataValue with a new constructor using read dataType
                dataValue = new DataValue(variant);
                //Create a WriteValue using the NodeId, dataValue and attributeType
                WriteValue valueToWrite = new WriteValue();
                valueToWrite.Value = dataValue;
                valueToWrite.NodeId = nodeId;
                valueToWrite.AttributeId = Attributes.Value;

                //Add the dataValues to the collection
                valuesToWrite.Add(valueToWrite);
            }
            try
            {
                //Write the collection to the server
                _mSession.Write(null, valuesToWrite, out result, out diagnostics);
                returnVal = result[0].ToString();
                return returnVal;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;

            }

        }

        /// <summary>
        /// 读取单个点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeIdString"></param>
        /// <returns></returns>
        public T ReadValue<T>(string nodeIdString)
        {
            //Create a nodeId using the identifier string
            NodeId nodeId = new NodeId(nodeIdString);
            try
            {
                //Read the dataValue
                DataValue dataValue = _mSession.ReadValue(nodeId);
                return ChangeTo<T>(dataValue.Value);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        private static T ChangeTo<T>(object obj)
        {
            if (typeof(T) == typeof(Guid))//Guid不能直接转换
            {
                return (T)Convert.ChangeType(new Guid(obj.ToString()), typeof(T));
            }
            T result = default(T);
            result = (T)Convert.ChangeType(obj, typeof(T));
            return result;
        }

        /// <summary>Reads values from node Ids</summary>
        /// <param name="nodeIdStrings">The node Ids as string</param>
        /// <returns>The read values as strings</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public List<string> ReadValues(List<string> nodeIdStrings)
        {
            List<NodeId> nodeIds = new List<NodeId>();
            List<Type> types = new List<Type>();
            List<object> values = new List<object>();
            List<ServiceResult> serviceResults = new List<ServiceResult>();
            foreach (string str in nodeIdStrings)
            {
                //Create a nodeId using the identifier string and add to list
                nodeIds.Add(new NodeId(str));
                //No need for types
                types.Add(null);
            }
            try
            {
                //Read the dataValues
                _mSession.ReadValues(nodeIds, types, out values, out serviceResults);
                //check ServiceResults to 
                foreach (ServiceResult svResult in serviceResults)
                {
                    if (svResult.ToString() != "Good")
                    {
                        Exception e = new Exception(svResult.ToString());
                        throw e;
                    }
                }
                List<string> resultStrings = new List<string>();
                foreach (object result in values)
                {
                    if (result.ToString() == "System.Byte[]")
                    {
                        string str = "";
                        str = BitConverter.ToString((byte[])result).Replace("-", ";");
                        resultStrings.Add(str);
                    }
                    else if (result.ToString() == "System.Boolean[]")
                    {
                        string str = "";
                        foreach (Boolean intVar in (Boolean[])result)
                        {
                            str = str + ";" + intVar.ToString();
                        }
                        str = str.Remove(0, 1);
                        resultStrings.Add(str);
                    }
                    else if (result.ToString() == "System.Int16[]")
                    {
                        string str = "";
                        foreach (Int16 intVar in (Int16[])result)
                        {
                            str = str + ";" + intVar.ToString();
                        }
                        str = str.Remove(0, 1);
                        resultStrings.Add(str);
                    }
                    else if (result.ToString() == "System.UInt16[]")
                    {
                        string str = "";
                        foreach (UInt16 intVar in (UInt16[])result)
                        {
                            str = str + ";" + intVar.ToString();
                        }
                        str = str.Remove(0, 1);
                        resultStrings.Add(str);
                    }
                    else if (result.ToString() == "System.Int64[]")
                    {
                        string str = "";
                        foreach (Int64 intVar in (Int64[])result)
                        {
                            str = str + ";" + intVar.ToString();
                        }
                        str = str.Remove(0, 1);
                        resultStrings.Add(str);
                    }
                    else if (result.ToString() == "System.Single[]")
                    {
                        string str = "";
                        foreach (float intVar in (float[])result)
                        {
                            str = str + ";" + intVar.ToString();
                        }
                        str = str.Remove(0, 1);
                        resultStrings.Add(str);
                    }
                    else
                    {
                        resultStrings.Add(result.ToString());
                    }
                }
                return resultStrings;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Registers Node Ids to the server</summary>
        /// <param name="nodeIdStrings">The node Ids as strings</param>
        /// <returns>The registered Node Ids as strings</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public List<string> RegisterNodeIds(List<string> nodeIdStrings)
        {
            NodeIdCollection nodesToRegister = new NodeIdCollection();
            NodeIdCollection registeredNodes = new NodeIdCollection();
            List<string> registeredNodeIdStrings = new List<string>();
            foreach (string str in nodeIdStrings)
            {
                //Create a nodeId using the identifier string and add to list
                nodesToRegister.Add(new NodeId(str));
            }
            try
            {
                //Register nodes
                _mSession.RegisterNodes(null, nodesToRegister, out registeredNodes);

                foreach (NodeId nodeId in registeredNodes)
                {
                    registeredNodeIdStrings.Add(nodeId.ToString());
                }

                return registeredNodeIdStrings;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Unregister Node Ids to the server</summary>
        /// <param name="nodeIdStrings">The node Ids as string</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void UnregisterNodeIds(List<string> nodeIdStrings)
        {
            NodeIdCollection nodesToUnregister = new NodeIdCollection();
            List<string> registeredNodeIdStrings = new List<string>();
            foreach (string str in nodeIdStrings)
            {
                //Create a nodeId using the identifier string and add to list
                nodesToUnregister.Add(new NodeId(str));
            }
            try
            {
                //Register nodes                
                _mSession.UnregisterNodes(null, nodesToUnregister);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        #endregion

        #region EventHandling
        /// <summary>Eventhandler to accept the server certificate forwards this event</summary>
        private void Validator_CertificateValidation(CertificateValidator certificate, CertificateValidationEventArgs e)
        {
            //Accept the certificate send from the server
            e.Accept = true;
        }

        /// <summary>Eventhandler for MonitoredItemNotifications forwards this event</summary>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (ItemChangedNotification != null)
            {
                NotificationEventInfo data = new NotificationEventInfo
                {
                    MonitoredItem=monitoredItem,
                    Args=e,
                };
                Task.Factory.StartNew(new Action<object>(obj =>
                {
                    NotificationEventInfo info = obj as NotificationEventInfo;
                    if (info == null)
                    {
                        return;
                    }
                    ItemChangedNotification(info.MonitoredItem, info.Args);
                }), data);
            }
        }

        /// <summary>Eventhandler for KeepAlive forwards this event</summary>
        private void Notification_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            if (KeepAliveNotification != null)
            {
                KeepAliveNotification(session, e);
            }
        }
        #endregion

        #region Private methods
        /// <summary>Creats a minimal required ApplicationConfiguration</summary>
        /// <param name="localIpAddress">The ip address of the interface to connect with</param>
        /// <returns>The ApplicationConfiguration</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private static ApplicationConfiguration CreateClientConfiguration()
        {
            // The application configuration can be loaded from any file.
            // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
            // This approach allows applications to share configuration files and to update them.
            // This example creates a minimum ApplicationConfiguration using its default constructor.
            ApplicationConfiguration configuration = new ApplicationConfiguration();

            // Step 1 - Specify the client identity.        
            configuration.ApplicationName = "UA Client";
            configuration.ApplicationType = ApplicationType.Client;
            configuration.ApplicationUri = "http://localhost/VendorId/ApplicationId/InstanceId";
            configuration.ProductUri = "http://VendorId/ProductId/VersionId";


            // Step 2 - Specify the client's application instance certificate.
            // Application instance certificates must be placed in a windows certficate store because that is 
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.            
            // When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

            configuration.SecurityConfiguration = new SecurityConfiguration();
            configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
            configuration.SecurityConfiguration.ApplicationCertificate.StoreType = "X509Store";
            configuration.SecurityConfiguration.ApplicationCertificate.StorePath = "LocalMachine\\My";
            configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = configuration.ApplicationName;

            //// find the client certificate in the store.
            //X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true).Result;

            //// Get local interface ip addresses and DNS name
            //List<string> localIps = GetLocalIPAddressAndDns();

            //// create a new certificate if one not found.
            //if (clientCertificate == null)
            //{
            //  // this code would normally be called as part of the installer - called here to illustrate.
            //  // create a new certificate an place it in the current directory.
            //  clientCertificate = CertificateFactory.CreateCertificate(
            //      configuration.SecurityConfiguration.ApplicationCertificate.StoreType,
            //      configuration.SecurityConfiguration.ApplicationCertificate.StorePath,"",
            //      configuration.ApplicationUri,
            //      configuration.ApplicationName,
            //      null,
            //      localIps,
            //      1024,
            //      120);
            //}


            // Step 3 - Specify the supported transport quotas.
            // The transport quotas are used to set limits on the contents of messages and are
            // used to protect against DOS attacks and rogue clients. They should be set to
            // reasonable values.
            configuration.TransportQuotas = new TransportQuotas();
            configuration.TransportQuotas.OperationTimeout = 360000;
            configuration.TransportQuotas.MaxStringLength = 67108864;

            // Step 4 - Specify the client specific configuration.
            configuration.ClientConfiguration = new ClientConfiguration();
            configuration.ClientConfiguration.DefaultSessionTimeout = 999999999;

            // Step 5 - Validate the configuration.

            // This step checks if the configuration is consistent and assigns a few internal variables
            // that are used by the SDK. This is called automatically if the configuration is loaded from
            // a file using the ApplicationConfiguration.Load() method.          
            configuration.Validate(ApplicationType.Client);

            return configuration;
        }

        /// <summary>Creats an EndpointDescription</summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="security">Use security or not</param>
        /// <returns>The EndpointDescription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private static EndpointDescription CreateEndpointDescription(string url, bool security)
        {
            // create the endpoint description.
            EndpointDescription endpointDescription = new EndpointDescription();

            // submit the url of the endopoint
            endpointDescription.EndpointUrl = url;

            // specify the security policy to use.
            if (security)
            {
                endpointDescription.SecurityPolicyUri = SecurityPolicies.Basic128Rsa15;
                endpointDescription.SecurityMode = MessageSecurityMode.SignAndEncrypt;
            }
            else
            {
                endpointDescription.SecurityPolicyUri = SecurityPolicies.None;
                endpointDescription.SecurityMode = MessageSecurityMode.None;
            }

            // specify the transport profile.
            endpointDescription.TransportProfileUri = Profiles.UaTcpTransport;

            return endpointDescription;
        }

        /// <summary>Gets the local IP addresses and the DNS name</summary>
        /// <returns>The list of IPs and names</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public static List<string> GetLocalIPAddressAndDns()
        {
            List<string> localIps = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIps.Add(ip.ToString());
                }

            }
            if (localIps.Count == 0)
            {
                throw new Exception("Local IP Address Not Found!");
            }
            localIps.Add(Dns.GetHostName());
            return localIps;
        }
        #endregion
    }

    internal class NotificationEventInfo
    {
        public MonitoredItem MonitoredItem { get; set; }
        public MonitoredItemNotificationEventArgs Args { get; set; }
    }
}