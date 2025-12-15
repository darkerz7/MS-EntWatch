namespace MS_EntWatch
{
    internal class Scheme
    {
        public string Color_tag { get; set; }
        public string Color_name { get; set; }
        public string Color_steamid { get; set; }
        public string Color_use { get; set; }
        public string Color_pickup { get; set; }
        public string Color_drop { get; set; }
        public string Color_disconnect { get; set; }
        public string Color_death { get; set; }
        public string Color_warning { get; set; }
        public string Color_enabled { get; set; }
        public string Color_disabled { get; set; }

        public string Server_name { get; set; }

        public Scheme()
        {
            Color_tag = "{green}";
            Color_name = "{default}";
            Color_steamid = "{grey}";
            Color_use = "{lightblue}";
            Color_pickup = "{lime}";
            Color_drop = "{pink}";
            Color_disconnect = "{orange}";
            Color_death = "{orange}";
            Color_warning = "{orange}";
            Color_enabled = "{green}";
            Color_disabled = "{red}";

            Server_name = "Zombies Server";
        }
    }
}
