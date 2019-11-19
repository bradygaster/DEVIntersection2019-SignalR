using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Streaming
{
    public class StreamHolder
    {
        public StreamHolder()
        {
            this.ChannelToHold = Channel.CreateBounded<Rotation>(1000);
        }

        public Channel<Rotation> ChannelToHold { get; set; }
    }

    public class RotationHub : Hub
    {
        private readonly ILogger<RotationHub> _logger;

        public StreamHolder StreamHolder { get; }

        public RotationHub(ILogger<RotationHub> logger, StreamHolder streamHolder)
        {
            _logger = logger;
            StreamHolder = streamHolder;
        }

        public async void Rotate(double x, double y, double z)
        {
            await Clients.Others.SendCoreAsync("rotated", new object[] { x, y, z });
        }

        public async Task UploadStream(ChannelReader<Rotation> stream)
        {
            await Task.Yield();
            
            while(await stream.WaitToReadAsync())
            {
                while(stream.TryRead(out var item))
                {
                    _ = StreamHolder.ChannelToHold.Writer.WriteAsync(item);
                }
            }
        }


        public IAsyncEnumerable<Rotation> DownloadStream(CancellationToken token)
        {
            _ = WriteDateAsync(StreamHolder.ChannelToHold.Writer, token);
            return StreamHolder.ChannelToHold.Reader.ReadAllAsync();
        }

        private async Task WriteDateAsync(ChannelWriter<Rotation> writer, 
            CancellationToken token)
        {
            try
            {
                while(true)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(10, token);
                }
            }
            catch
            {
                writer.TryComplete();
            }
            
            writer.TryComplete();
        }
    }

    public class Rotation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
