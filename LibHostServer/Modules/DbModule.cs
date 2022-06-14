using Kingsoft.Utils.Http;
using Kingsoft.Utils.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer.Modules
{
    public static class DbModule
    {
        public static void Init(HostServer _server)
        {
            // -> /db/<command>/<key>
            _server.Server.GetRoutes.Add(new Route("/db/get/<var>", (args, self, WriteRes, req) =>
            {
                Dictionary<string, string> query = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                string status = "500";
                object res = null;

                ServerRole _role = _server.data.Self.ServerType;

                ServerInfo _info;
                string result;

                if (query.ContainsKey(_server.keys[0]))
                {
                    switch (_role)
                    {
                        case ServerRole.standby:
                            break;
                        case ServerRole.input:
                            _info = _server.GetProcServer();
                            result = _info.Request($"/db/get/{args[0]}?{_server.keys[0]}={query[_server.keys[0]]}");
                            break;
                        case ServerRole.processing:
                            _info = _server.data.OutputServer;
                            result = _server.dataBase[args[0]];
                            result = _info.Request($"/db/add/{query[_server.keys[0]]}_res/{result}");
                            if ((string)result.jobj<Dictionary<string, object>>()["status"] == "300")
                                result = _info.Request($"/db/set/res_{query[_server.keys[0]]}/{result}");
                            status = (string)result.jobj<Dictionary<string, object>>()["status"];
                            break;
                        case ServerRole.backup:
                            break;
                        case ServerRole.output:
                            res = _server.dataBase[args[0]];
                            break;
                        case ServerRole.lonely:
                            if (!args[0].StartsWith("res_")) _server.dataBase[$"res_{query[_server.keys[0]]}"] = _server.dataBase[args[0]];
                            else if (_server.dataBase.ContainsKey(args[0])) res = _server.dataBase[args[0]];
                            break;
                        case ServerRole.inoutput:
                            _info = _server.data.ProcessingServers[0];
                            if (_server.dataBase.ContainsKey(args[0]) && !args[0].StartsWith("res_")) result = _info.Request($"/db/get/{args[0]}");
                            else if (_server.dataBase.ContainsKey(args[0]) && args[0].StartsWith("res_")) _server.dataBase[args[0]] = args[1];
                            break;
                        default:
                            break;
                    }
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status,
                    ["result"] = res
                }.json();

                Console.WriteLine(_server.dataBase.json());
                Console.WriteLine(_server.data.Self.ServerType.json());

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            // -> /db/<command>/<key>/<value>
            _server.Server.GetRoutes.Add(new Route("/db/set/<var>/<var>", (args, self, WriteRes, req) =>
            {
                Dictionary<string, string> query = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                string status = "500";
                object res = null;

                ServerRole _role = _server.data.Self.ServerType;

                ServerInfo _info;
                string result;

                if (query.ContainsKey(_server.keys[0]))
                {
                    switch (_role)
                    {
                        case ServerRole.standby:
                            break;
                        case ServerRole.input:
                            _info = _server.GetProcServer();
                            result = _info.Request($"/db/set/{args[0]}/{args[1]}?{_server.keys[0]}={query[_server.keys[0]]}");
                            break;
                        case ServerRole.processing:
                            _info = _server.data.OutputServer;
                            result = _info.Request($"/db/set/res_{query[_server.keys[0]]}/{args[1]}?{_server.keys[0]}={query[_server.keys[0]]}");
                            break;
                        case ServerRole.backup:
                            break;
                        case ServerRole.output:
                            if (!args[0].StartsWith("res_")) _server.dataBase[args[0]] = args[1];
                            _server.dataBase[$"res_{query[_server.keys[0]]}"] = "200";
                            break;
                        case ServerRole.lonely:
                            _server.dataBase[args[0]] = args[1];
                            _server.dataBase[$"res_{query[_server.keys[0]]}"] = "200";
                            break;
                        case ServerRole.inoutput:
                            _server.dataBase[args[0]] = args[1];
                            break;
                        default:
                            break;
                    }
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                Console.WriteLine(_server.dataBase.json());
                Console.WriteLine(_server.data.Self.ServerType.json());

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            // -> /db/<command>/<key>/<value>
            _server.Server.GetRoutes.Add(new Route("/db/add/<var>/<var>", (args, self, WriteRes, req) =>
            {
                Dictionary<string, string> body = HttpServer.Utils.GetFPostBody(req);
                string status = "300";

                if (!_server.dataBase.ContainsKey(args[0]))
                {
                    _server.dataBase[args[0]] = args[1];
                    status = "200";
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                Console.WriteLine(_server.dataBase.json());
                Console.WriteLine(_server.data.Self.ServerType.json());

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));
        }
    }
}
