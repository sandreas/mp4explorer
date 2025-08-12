using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace mp4explorer.Models;

public class Atom
{
    public string Name { get; set; }
    public long Position { get; set; } = 0;
    public long Size { get; set; } = 0;
    public long End => Position + Size;
    public List<Atom> Children { get; set; }
    

    public Atom(string name, long position, long size, List<Atom>? children = null)
    {
        Name = name;
        Position = position;
        Size = size;
        Children= children ?? [];
    }

    public void AddChild(Atom child)
    {
        Children.Add(child);
    }
}