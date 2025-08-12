using mp4explorer.Readers;

namespace mp4explorer.Tests.Unit.Readers;

public class Mp4AtomReaderTest {
    private readonly Mp4AtomReader _subject;

    public Mp4AtomReaderTest()
    {
        _subject = new Mp4AtomReader();
    }

    [Fact]
    public void Test1()
    {
        var fs = File.OpenRead("/home/andreas/Downloads/Das Siegel von Rapgar/Das Siegel von Rapgar.m4b");
        using (var reader = new BinaryReader(fs))
        {
            var atom = _subject.ReadAtoms(reader);
        }
        
    }
}