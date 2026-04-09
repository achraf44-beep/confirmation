namespace Domain.Locations;

public sealed record Wilaya(int Code, string Name, string NameAr);

public sealed record Commune(int WilayaCode, string Name, string NameAr);

public static class AlgerianWilayas
{
    public static readonly IReadOnlyList<Wilaya> All = new List<Wilaya>
    {
        new(1, "Adrar", "أدرار"),
        new(2, "Chlef", "الشلف"),
        new(3, "Laghouat", "الأغواط"),
        new(4, "Oum El Bouaghi", "أم البواقي"),
        new(5, "Batna", "باتنة"),
        new(6, "Béjaïa", "بجاية"),
        new(7, "Biskra", "بسكرة"),
        new(8, "Béchar", "بشار"),
        new(9, "Blida", "البليدة"),
        new(10, "Bouira", "البويرة"),
        new(11, "Tamanrasset", "تمنراست"),
        new(12, "Tébessa", "تبسة"),
        new(13, "Tlemcen", "تلمسان"),
        new(14, "Tiaret", "تيارت"),
        new(15, "Tizi Ouzou", "تيزي وزو"),
        new(16, "Alger", "الجزائر"),
        new(17, "Djelfa", "الجلفة"),
        new(18, "Jijel", "جيجل"),
        new(19, "Sétif", "سطيف"),
        new(20, "Saïda", "سعيدة"),
        new(21, "Skikda", "سكيكدة"),
        new(22, "Sidi Bel Abbès", "سيدي بلعباس"),
        new(23, "Annaba", "عنابة"),
        new(24, "Guelma", "قالمة"),
        new(25, "Constantine", "قسنطينة"),
        new(26, "Médéa", "المدية"),
        new(27, "Mostaganem", "مستغانم"),
        new(28, "M'Sila", "المسيلة"),
        new(29, "Mascara", "معسكر"),
        new(30, "Ouargla", "ورقلة"),
        new(31, "Oran", "وهران"),
        new(32, "El Bayadh", "البيض"),
        new(33, "Illizi", "إليزي"),
        new(34, "Bordj Bou Arréridj", "برج بوعريريج"),
        new(35, "Boumerdès", "بومرداس"),
        new(36, "El Tarf", "الطارف"),
        new(37, "Tindouf", "تندوف"),
        new(38, "Tissemsilt", "تيسمسيلت"),
        new(39, "El Oued", "الوادي"),
        new(40, "Khenchela", "خنشلة"),
        new(41, "Souk Ahras", "سوق أهراس"),
        new(42, "Tipaza", "تيبازة"),
        new(43, "Mila", "ميلة"),
        new(44, "Aïn Defla", "عين الدفلى"),
        new(45, "Naâma", "النعامة"),
        new(46, "Aïn Témouchent", "عين تموشنت"),
        new(47, "Ghardaïa", "غرداية"),
        new(48, "Relizane", "غليزان"),
        new(49, "El M'Ghair", "المغير"),
        new(50, "El Meniaa", "المنيعة"),
        new(51, "Ouled Djellal", "أولاد جلال"),
        new(52, "Bordj Badji Mokhtar", "برج باجي مختار"),
        new(53, "Béni Abbès", "بني عباس"),
        new(54, "Timimoun", "تيميمون"),
        new(55, "Touggourt", "تقرت"),
        new(56, "Djanet", "جانت"),
        new(57, "In Salah", "عين صالح"),
        new(58, "In Guezzam", "عين قزام")
    };

    public static Wilaya? GetByCode(int code) => All.FirstOrDefault(w => w.Code == code);
    public static Wilaya? GetByName(string name) => All.FirstOrDefault(w => 
        w.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || 
        w.NameAr.Equals(name, StringComparison.OrdinalIgnoreCase));
}
