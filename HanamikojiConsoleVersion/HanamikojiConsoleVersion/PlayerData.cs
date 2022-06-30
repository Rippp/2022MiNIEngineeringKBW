﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiConsoleVersion;
public class PlayerData
{
    public List<GiftCard> CardsOnHand { get; set; } = new List<GiftCard>();
    public List<GiftCard> GiftsFromPlayer { get; set; } = new List<GiftCard>();
    public GiftCard? SecretCard { get; set; } = null;
}
