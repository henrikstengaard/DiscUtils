namespace LibraryTests.Vhd
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using DiscUtils;
    using DiscUtils.CoreCompat;
    using DiscUtils.Iscsi;
    using DiscUtils.Streams;
    using Xunit;

    public class GivenVhd
    {
        [Fact]
        public async Task WhenCreateAndReadVhdThenContentIsIdentical()
        {
            // arrange
            var bytes = new byte[10 * 1024 * 1024];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(i % 255);
            }
            var path = "blank.vhd";
            
            // act
            await CreateVhd(path, bytes);
            
            // assert
            var actualBytes = await ReadVhd(path);
            Assert.Equal(bytes, actualBytes);
        }

        private async Task CreateVhd(string path, byte[] bytes)
        {
            await using var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            var vhdDisk = DiscUtils.Vhd.Disk.InitializeDynamic(stream, Ownership.None, bytes.Length);
            vhdDisk.Content.Position = 0;
            await vhdDisk.Content.WriteAsync(bytes, 0, bytes.Length);
        }

        private async Task<byte[]> ReadVhd(string path)
        {
            DiscUtils.Setup.SetupHelper.RegisterAssembly(ReflectionHelper.GetAssembly(typeof(Disk)));
            VirtualDiskManager.RegisterVirtualDiskTypes(typeof(DiscUtils.Vhd.Disk).Assembly);
            var vhdDisk = VirtualDisk.OpenDisk(path, FileAccess.Read);

            var bytes = new byte[vhdDisk.Capacity];
            vhdDisk.Content.Position = 0;
            await vhdDisk.Content.ReadAsync(bytes, 0, bytes.Length);

            return bytes;
        }
    }
}