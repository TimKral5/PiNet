using Kingsoft.Utils.Http;
using Kingsoft.Utils.TypeExtensions;
using Kingsoft.Utils.TypeExtensions.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer.Modules
{
    public static class ServerModule
    {
        public static void Init(HostServer _server)
        {
            _server.Server.GetRoutes.Add(new Route("/server/join", (args, self, WriteRes, req) =>
            {

                string status = "500";

                Dictionary<string, string> query = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                if (query.ContainsKeys(_server.keys[0]))
                {
                    bool x = _server.GetHost(query[_server.keys[0]]).Item1;
                    if (!x)
                    {
                        _server.data.AddHost(new ServerInfo()
                        {
                            Address = query[_server.keys[0]],
                            ServerType = ServerRole.processing
                        });

                        status = "200";
                    }
                    else
                    {
                        status = "300";
                    }
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            _server.Server.GetRoutes.Add(new Route("/server/getLogs", (args, self, WriteRes, req) =>
            {

                string status = "500";

                Dictionary<string, string> body = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                List<QueueItem> _data = null;
                if (body.ContainsKeys(_server.keys[0]))
                {
                    (bool, ServerInfo) _res = _server.GetHost(body[_server.keys[0]]);
                    bool x = _res.Item1;
                    ServerInfo _info = _res.Item2;
                    if (x)
                    {
                        _data = _info.Queue;

                        status = "200";
                    }
                    else
                    {
                        status = "300";
                    }
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status,
                    ["data"] = _data
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            _server.Server.GetRoutes.Add(new Route("/server/clearLogs", (args, self, WriteRes, req) =>
            {
                string status = "500";
                Dictionary<string, string> body = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                List<QueueItem> _data = null;
                if (body.ContainsKeys(_server.keys[0]))
                {
                    (bool, ServerInfo) _res = _server.GetHost(body[_server.keys[0]]);
                    bool x = _res.Item1;
                    ServerInfo _info = _res.Item2;
                    if (x)
                    {
                        _data = _info.Queue = new List<QueueItem>();
                        status = "200";
                    }
                    else
                    {
                        status = "300";
                    }
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status,
                    ["data"] = _data
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));
        }
    }
}
