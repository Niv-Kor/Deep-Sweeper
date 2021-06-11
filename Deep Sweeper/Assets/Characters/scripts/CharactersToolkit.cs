namespace DeepSweeper.Characters
{
    public static class CharactersToolkit
    {
        public static TribalRace Race(this Persona character) {
            switch (character) {
                case Persona.Alana: return TribalRace.Human;
                case Persona.Simon: return TribalRace.Human;
                case Persona.Fox: return TribalRace.Fox;
                case Persona.Adam: return TribalRace.Human;
                default: return TribalRace.None;
            }
        }
    }
}