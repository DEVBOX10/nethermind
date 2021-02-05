﻿//  Copyright (c) 2021 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Extensions;
using Nethermind.Int256;

namespace Nethermind.Serialization.Rlp
{
    public class AccessListDecoder : IRlpStreamDecoder<AccessList?>
    {
        public AccessList? Decode(RlpStream rlpStream, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
        {
            if (rlpStream.IsNextItemNull())
            {
                rlpStream.ReadByte();
                return null;
            }

            int length = rlpStream.PeekNextRlpLength();
            int check = rlpStream.Position + length;
            HashSet<Address> accounts = new();
            HashSet<StorageCell> storageCells = new();
            AccessList accessList = new(accounts, storageCells);
            while (rlpStream.Position <= check)
            {
                int lengthOfNextAddress = rlpStream.PeekNextRlpLength();
                if (lengthOfNextAddress == 1)
                {
                    break;
                }

                rlpStream.SkipLength();
                Address address = rlpStream.DecodeAddress();
                if (address == null)
                {
                    throw new RlpException("Invalid tx access list format - address is null");
                }

                accounts.Add(address);
                rlpStream.SkipLength();
                while (rlpStream.Position < check)
                {
                    int lengthOfNextItemInStorage = rlpStream.PeekNextRlpLength();
                    if (lengthOfNextItemInStorage == Rlp.LengthOfKeccakRlp)
                    {
                        UInt256 index = rlpStream.DecodeUInt256();
                        StorageCell storageCell = new(address, index);
                        storageCells.Add(storageCell);
                    }
                    else if (lengthOfNextItemInStorage == 1)
                    {
                        rlpStream.ReadByte();
                        break;
                        // ?
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if ((rlpBehaviors & RlpBehaviors.AllowExtraData) != RlpBehaviors.AllowExtraData)
            {
                rlpStream.Check(check);
            }

            return accessList;
        }

        public void Encode(RlpStream stream, AccessList? item, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
        {
            if (item == null)
            {
                stream.WriteByte(Rlp.NullObjectByte);
            }
        }

        private static int GetContentLength(AccessList? item, RlpBehaviors rlpBehaviors)
        {
            if (item is null)
            {
                return 0;
            }
            
            throw new NotImplementedException();
        }

        public int GetLength(AccessList? item, RlpBehaviors rlpBehaviors)
        {
            if (item is null)
            {
                return 0;
            }
            
            throw new NotImplementedException();
        }
    }
}
