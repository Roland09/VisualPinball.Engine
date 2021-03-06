﻿// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

#region ReSharper
// ReSharper disable UnassignedField.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualPinball.Engine.IO;
using VisualPinball.Engine.VPT.Table;

namespace VisualPinball.Engine.VPT.Mappings
{
	[Serializable]
	public class MappingsData : ItemData
	{
		public override string GetName() => Name;
		public override void SetName(string name) { Name = name; }

		[BiffString("NAME", IsWideString = true, Pos = 1)]
		public string Name = string.Empty;

		[BiffMappingsSwitchAttribute("MSWT", TagAll = true, Pos = 1000)]
		public MappingsSwitchData[] Switches = Array.Empty<MappingsSwitchData>();

		[BiffMappingsCoilAttribute("MCOI", TagAll = true, Pos = 1001)]
		public MappingsCoilData[] Coils = Array.Empty<MappingsCoilData>();

		[BiffMappingsWireAttribute("MWIR", TagAll = true, Pos = 1002)]
		public MappingsWireData[] Wires = Array.Empty<MappingsWireData>();

		[BiffMappingsLampAttribute("MLMP", TagAll = true, Pos = 1003)]
		public MappingsLampData[] Lamps = Array.Empty<MappingsLampData>();

		#region BIFF

		static MappingsData()
		{
			Init(typeof(MappingsData), Attributes);
		}

		public MappingsData(string name) : base(StoragePrefix.Mappings)
		{
			Name = name;
		}

		public MappingsData(BinaryReader reader, string storageName) : base(storageName)
		{
			Load(this, reader, Attributes);
		}

		public override void Write(BinaryWriter writer, HashWriter hashWriter)
		{
			WriteRecord(writer, Attributes, hashWriter);
			WriteEnd(writer, hashWriter);
		}

		private static readonly Dictionary<string, List<BiffAttribute>> Attributes = new Dictionary<string, List<BiffAttribute>>();

		#endregion

		#region Switches

		public void AddSwitch(MappingsSwitchData data)
		{
			Switches = Switches.Append(data).ToArray();
		}

		public void RemoveSwitch(MappingsSwitchData data)
		{
			Switches = Switches.Except(new[] { data }).ToArray();
		}

		public void RemoveAllSwitches()
		{
			Switches = Array.Empty<MappingsSwitchData>();
		}

		#endregion

		#region Coils

		public void AddCoil(MappingsCoilData data)
		{
			Coils = Coils.Append(data).ToArray();
		}

		public void RemoveCoil(MappingsCoilData data)
		{
			Coils = Coils.Except(new[] { data }).ToArray();
			if (data.Destination == CoilDestination.Lamp) {
				Lamps = Lamps.Where(l => l.Id == data.Id && l.Source == LampSource.Coils).ToArray();
			}
		}

		public void RemoveAllCoils()
		{
			Coils = Array.Empty<MappingsCoilData>();
			Lamps = Lamps.Where(l => l.Source != LampSource.Coils).ToArray();
		}

		#endregion

		#region Wires

		public void AddWire(MappingsWireData data)
		{
			Wires = Wires.Append(data).ToArray();
		}

		public void RemoveWire(MappingsWireData data)
		{
			Wires = Wires.Except(new[] { data }).ToArray();
		}

		public void RemoveAllWires()
		{
			Wires = Array.Empty<MappingsWireData>();
		}

		#endregion

		#region Lamps

		public void AddLamp(MappingsLampData data)
		{
			Lamps = Lamps.Append(data).ToArray();
		}

		public void RemoveLamp(MappingsLampData data)
		{
			Lamps = Lamps.Except(new[] { data }).ToArray();
		}

		public void RemoveAllLamps()
		{
			Lamps = Lamps.Where(l => l.Source == LampSource.Coils).ToArray();
		}

		#endregion
	}
}
