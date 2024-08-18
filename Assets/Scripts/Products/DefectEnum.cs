using System;

[Flags]
public enum DefectType
{
    None = 0,
    
    Hex_MissingMaterial_1 = 1 << 0,
    Hex_MissingMaterial_2 = 1 << 1,
    Hex_MissingMaterial_3 = 1 << 2,
    Hex_MissingMaterial_4 = 1 << 3,
    Hex_MissingMaterial_5 = 1 << 4,
    
    E_No_Cutout = 1 << 10,
    E_Made_6 = 1 << 11,
}