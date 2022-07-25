using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Shared
{
    [Serializable]
    public class PersonKey
    {
        private string[] names = new string[] { "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Lisa", "Daniel", "Nancy", " Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra", "Donald", "Ashley", "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle", "Kenneth", "Carol", "Kevin", "Amanda", "Brian", "Dorothy", "George", "Melissa", "Timothy", "Deborah", "Ronald", "Stephanie", "Edward", "Rebecca", "Jason", "Sharon", "Jeffrey", "Laura", "Ryan", "Cynthia", "Jacob", "Kathleen", "Gary", "Amy", "Nicholas", "Angela", "Eric", "Shirley", "Jonathan", "Anna", "Stephen", "Brenda", "Larry", "Pamela", "Justin", "Emma", "Scott", "Nicole", "Brandon", "Helen", "Benjamin", "Samantha", "Samuel", "Katherine", "Gregory", "Christine", "Alexander", "Debra", "Frank", "Rachel", "Patrick", "Carolyn", "Raymond", "Janet", "Jack", "Catherine", "Dennis", "Maria", "Jerry", "Tyler", "Aaron", "Jose", "Adam", "Nathan", "Henry", "Douglas", "Zachary", "Peter", "Kyle", "Ethan", "Walter", "Noah", "Jeremy", "Christian", "Keith", "Roger", "Terry", "Gerald", "Harold", "Sean", "Austin", "Carl", "Arthur", "Lawrence", "Dylan", "Jesse", "Jordan", "Bryan", "Billy", "Joe", "Bruce", "Gabriel", "Logan", "Albert", "Willie", "Alan", "Juan", "Wayne", "Elijah", "Randy", "Roy", "Vincent", "Ralph", "Eugene", "Russell", "Bobby", "Mason", "Philip", "Louis", "Heather", "Diane", "Ruth", "Julie", "Olivia", "Joyce", "Virginia", "Victoria", "Kelly", "Lauren", "Christina", "Joan", "Evelyn", "Judith", "Megan", "Andrea", "Cheryl", "Hannah", "Jacqueline", "Martha", "Gloria", "Teresa", "Ann", "Sara", "Madison", "Frances", "Kathryn", "Janice", "Jean", "Abigail", "Alice", "Julia", "Judy", "Sophia", "Grace", "Denise", "Amber", "Amber", "Doris", "Marilyn", "Danielle", "Beverly", "Isabella", "Theresa", "Diana", "Natalie", "Brittany", "Charlotte", "Marie", "Kayla", "Alexis", "Loris", "Solenn", "Bonnie", "Chiara", "Matthias", "Lucas", "Marcel", "Till", "Robin" };
        private string[] vegetables = new string[] { "Potato", "Broccoli", "Garlic", "Onion", "Tomato", "Corn", "Mushroom", "Carrot", "Kale", "Amaranth", "Arrowroot", "Artichoke", "Asparagus", "Acerola", "Apple", "Apricot", "Avocado", "Bamboo", "Beetroot", "Bell Pepper", "Black Eyed Pea", "Radish", "Bok Choy", "Brussel Sprout", "Butternut Squash", "Banana", "Blackberry", "Blueberry", "Cabbage", "Cactus", "Caper", "Cauliflower", "Celery", "Cherry Tomato", "Chickpea", "Chickweed", "Chicory", "Chives", "Chrysanthemum", "Cress", "Cucumber", "Cantaloupe", "Cherries", "Clementine", "Coconut", "Cranberry", "Dandelion", "Dill", "Dulse", "Date", "Durian", "Eggplant", "Endive", "Elderberrry", "Fat Hen", "Fennel", "Fig", "Garbanzo", "Garden Rocket", "Ginger", "Green Bean", "Grapefruit", "Guava", "Soybean", "Honeydew Melon", "Jackfruit", "Kohlrabi", "Laver", "Leek", "Lemongrass", "Lemon", "Lentil", "Lettuce", "Lotus Root", "Spinach", "Mangetout", "Moth Bean", "Mung Bean", "Mustard", "Nori", "Pak Choy", "Parsnip", "Pea", "Pigeon Pea", "Pumpkin", "Plum", "Kiwi", "Lime", "Lychee", "Mandarin", "Mango", "Nectarine", "Olive", "Orange", "Papaya", "Passion Fruit", "Peach", "Pear", "Dragonfruit", "Pineapple", "Pomegranate", "Raspberry", "Rhubarb", "Strawberry", "Watermelon", "Radicchio", "Rice bean", "Salad", "Grape", "Shallot", "Sorrel", "Sour Cabbage", "Scallion", "Swede", "Sweet Potato", "Tigernut", "Turmeric", "Turnip", "Tomatillo", "Wasabi", "Water Chestnut", "Zucchini", "Aloe Vera", "Anis", "Basil", "Borage", "Calendula", "Chamomile", " Chive", "Coriander", "Dill", "Ginkgo", "Ginseng", "Marjoram", "Oregano", "Parsley", "Peppermint", "Rosemary", "Sage", "Tarragon", "Thyme", "Valerian", "Mint", "Pepper", "Hibiscus", "Laurel" };
        private string[] adjectives = new string[] { "adorable", "adventurous", "aggressive", "alert", "amused", "angry", "annoyed", "annoying", "romantic", "spiritual", "disgusting", "shy", "miniature", "gorgeous", "grey", "lucky", "awesome", "proud", "icy", "majestic", "freezing", "spooky", "caring", "orange", "disgusted", "uninterested", "high - pitched", "curved", "remarkable", "important", "adventurous", "gratis", "selfish", "filthy", "wet", "thick", "sick", "odd", "weird", "kind - hearted", "rough", "good", "polite", "heavy", "married", "overrated", "easy", "nasty", "lame", "special", "jealous", "hypnotic", "modern", "needy", "thirsty", "private", "tasteless", "frightened", "famous", "hard", "important", "mature", "shaky", "fluffy", "quiet", "colourful", "trashy", "invincible", "fast", "rebel", "disastrous", "breakable", "voiceless", "crazy", "plastic", "smooth", "rotten", "incompetent", "polite", "devilish", "salty", "nervous", "enormous", "premium", "awful", "amazing", "elegant", "blushing", "anxious", "bad", "medical", "ugliest", "wild", "typical", "dirty", "sour", "clever", "enchanted", "uncovered", "better", "precious", "animated", "resonant", "abnormal", "loud", "thoughtful", "colossal", "quickest", "different", "acidic", "determined", "mighty", "happy", "tired", "unhealthy", "striped", "strong", "arrogant", "fancy", "cold", "watery", "smelly", "mean", "pale", "healthy", "talented", "macho", "rare", "homeless", "thin", "naughty", "creepy", "common", "rich", "delicious", "chemical", "fearless", "accurate", "dry", "imperfect", "violent", "meaty", "excited", "boring", "tall", "bizarre", "futuristic", "fresh", "obese", "sparkling", "successful", "ordinary", "lean", "sticky", "huge", "roasted", "unnatural", "lonely", "extra-large", "fragile", "oval", "chubby", "horrible", "juicy", "unknown", "curious", "educated", "peaceful", "political", "grumpy" };

        public int Key { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// Create a new instance of key person.
        /// </summary>
        public PersonKey()
        {
            Random r = new Random();

            int nameNumber = r.Next(0, names.Length);
            int vegetableNumber = r.Next(0, vegetables.Length);
            int adjectivesNumber = r.Next(0, adjectives.Length);

            Key = (nameNumber + 100) + (vegetableNumber + 100) * 1000 + (+adjectivesNumber) * 1000 * 1000;
            Name = names[nameNumber] + " the " + adjectives[adjectivesNumber] + " " + vegetables[vegetableNumber];
        }
    }
}
