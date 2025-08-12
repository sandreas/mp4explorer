using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mp4explorer.Models;

namespace mp4explorer.Readers;

public class Mp4AtomReader
{

    public static List<string> DefinedAtomTypes = [
            "ftyp",
            "free",
            "mdat",
            "moov",
            "mvhd",
            "trak",
            "tkhd",
            "udta",
            "edts",
            "elst",
            "mdia",
            "mdhd",
            "hdlr",
            "minf",
            "smhd",
            "dinf",
            "dref",
            "stbl",
            "stsd",
            "stts",
            "stsc",
            "stsz",
            "stco",
            "sgpd",
            "roll",
            "sbgp",
            "tref",
            "chap",
            "meta",
            "chpl",
            "gmhd",
            "gmin",
            "text",
            
        ];
    public Atom ReadAtoms(BinaryReader reader) {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var baseAtom = new Atom("root", 0, reader.BaseStream.Length);
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ReadSubAtoms(reader, baseAtom);
        }
        return baseAtom;
    }

    private void ReadSubAtoms(BinaryReader reader, Atom baseAtom)
    {
        while (reader.BaseStream.Position < baseAtom.End)
        {
            var position = reader.BaseStream.Position;
            var atomSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            var atomTypeBytes = reader.ReadBytes(4);
            var atomType = Encoding.Default.GetString(atomTypeBytes);
            var atomSize = BitConverter.ToInt32(atomSizeBytes, 0);


            if (atomSize > 0 && DefinedAtomTypes.Contains(atomType) && position + atomSize < baseAtom.Position + baseAtom.Size)
            {
                var subAtom = new Atom(atomType, position, atomSize);
                ReadSubAtoms(reader, subAtom);
                baseAtom.AddChild(subAtom);
            } else
            {
                reader.BaseStream.Seek(baseAtom.End, SeekOrigin.Begin);
            }
        }
        
    }
}