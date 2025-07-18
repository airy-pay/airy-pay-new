﻿using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class FinPay
{
    [YamlMember(typeof(int), Alias = "shopId")]
    public required int ShopId { get; set; }
    
    [YamlMember(typeof(string), Alias = "key1")]
    public required string Key1 { get; set; }
    
    [YamlMember(typeof(string), Alias = "key2")]
    public required string Key2 { get; set; }
}