using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace wolf3d_map
{
    public class DosBoxApi
    {
        private readonly HttpClient client;

        public DosBoxApi(string host, int port)
        {
            var baseAddress = $"http://{host}:{port}/api/v1/";
            client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        public async Task<byte[]> ReadMemory(uint offset, uint length)
        {
            try
            {
                byte[] output = await client.GetByteArrayAsync($"memory/{offset}/{length}");
                return output;
            }
            catch
            {
                return new byte[length];
            }
        }
        public async Task<byte> ReadByte(uint offset)
        {
            try
            {
                byte[] output = await client.GetByteArrayAsync($"memory/{offset}/1");
                return output[0];
            }
            catch
            {
                return 0;
            }
        }

    }
}