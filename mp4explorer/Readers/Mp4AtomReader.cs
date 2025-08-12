using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mp4explorer.Models;

namespace mp4explorer.Readers;

public class Mp4AtomReader
{
    // ©,-,a-z,A-Z
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
            "hldr",
            "ilst",
            "©ART",
            "data",
            "©nam",
            "©alb",
            "©gen",
            "©wrt",
            "cprt",
            "desc",
            "ldes",
            "©cmt",
            "©day",
            "©too",
            "purd",
            "stik",
            "pgap",
            "©nrt",
            "----",
            "covr",
            "mean",
            "name",
            "©mvn",
            "soal",
            "soal",
            
        ];
    public Atom ReadAtoms(BinaryReader reader) {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var baseAtom = new Atom("root", 0, reader.BaseStream.Length);
        ReadSubAtoms(reader, baseAtom);
        return baseAtom;
    }

    private string DecodeAtomName(byte[] bytes)
    {
        return Encoding.Latin1.GetString(bytes);

        if (bytes[0] == 0xA9)
        {
            return "©" + Encoding.ASCII.GetString(bytes.Skip(1).ToArray());
        }

        return Encoding.ASCII.GetString(bytes);
    }
    
    private void ReadSubAtoms(BinaryReader reader, Atom baseAtom, bool asList = false)
    {
        /*
        if (baseAtom.Name == "meta")
        {
            File.WriteAllBytes("/home/andreas/Downloads/Das Siegel von Rapgar/meta-dump.dat", reader.ReadBytes((int)(baseAtom.End - baseAtom.Position)));
            reader.BaseStream.Seek(baseAtom.Position, SeekOrigin.Begin);
        }
        */
        while (reader.BaseStream.Position < baseAtom.End)
        {
            var position = reader.BaseStream.Position;
            var atomSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            var atomTypeBytes = reader.ReadBytes(4);
            var atomType = DecodeAtomName(atomTypeBytes);
            var atomSize = BitConverter.ToInt32(atomSizeBytes, 0);

            var subAtom = new Atom(atomType, position, atomSize);
            if (atomType == "meta")
            {
                subAtom.Version = reader.ReadByte();
                subAtom.Flags = reader.ReadBytes(3);
            }
            

            if (atomSize > 0 && IsAtomTypeSupported(atomType) && subAtom.End <= baseAtom.End)
            {
                ReadSubAtoms(reader, subAtom, asList);
                baseAtom.AddChild(subAtom);
            } else
            {
                reader.BaseStream.Seek(baseAtom.End, SeekOrigin.Begin);
            }
        }
        
    }

    private static bool IsAtomTypeSupported(string atomType)
    {
        return atomType.All(c => char.IsLetter(c) || c == '-' || c == '©');

    }
}