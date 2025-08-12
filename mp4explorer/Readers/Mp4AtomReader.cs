using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mp4explorer.Models;

namespace mp4explorer.Readers;

public class Mp4AtomReader
{
    public Atom ReadAtoms(BinaryReader reader) {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var baseAtom = new Atom("root", 0, reader.BaseStream.Length);
        ReadSubAtoms(reader, baseAtom);
        return baseAtom;
    }

    private string DecodeAtomName(byte[] bytes)
    {
        // todo: this works for my test files, but does it always work?
        return Encoding.Latin1.GetString(bytes);

        if (bytes[0] == 0xA9)
        {
            return "©" + Encoding.ASCII.GetString(bytes.Skip(1).ToArray());
        }

        return Encoding.ASCII.GetString(bytes);
    }
    
    private void ReadSubAtoms(BinaryReader reader, Atom baseAtom, bool asList = false)
    {
        while (reader.BaseStream.Position < baseAtom.End)
        {
            var position = reader.BaseStream.Position;
            var atomSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            var atomNameBytes = reader.ReadBytes(4);
            var atomName = DecodeAtomName(atomNameBytes);
            var atomSize = BitConverter.ToInt32(atomSizeBytes, 0);

            var subAtom = new Atom(atomName, position, atomSize);
            
            // meta has to be handled differently
            if (atomName == "meta")
            {
                subAtom.Version = reader.ReadByte();
                subAtom.Flags = reader.ReadBytes(3);
            }
            
            if (atomSize > 0 && IsAtomTypeSupported(atomName) && subAtom.End <= baseAtom.End)
            {
                ReadSubAtoms(reader, subAtom, asList);
                baseAtom.AddChild(subAtom);
            } else
            {
                reader.BaseStream.Seek(baseAtom.End, SeekOrigin.Begin);
            }
        }
        
    }

    private static bool IsAtomTypeSupported(string atomType) => atomType.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '©');
}