using System;
using System.Collections.Generic;

public class RandomNameGenerator
{
    private readonly List<string> _allNames = new List<string>
    {
        "Aaron", "Abby", "Adam", "Aiden", "Alan", "Alex", "Alice", "Allison", "Amber", "Andrew", "Angela", "Anna",
        "Anthony", "Aravind", "Ashley", "Austin", "Bao", "Barbara", "Ben", "Beth", "Brandon", "Brenda", "Brian", "Brittany", "Bryant",
        "Caleb", "Cameron", "Carl", "Caroline", "Carter", "Catherine", "Charles", "Charlie", "Chloe", "Chris",
        "Christian", "Christina", "Christopher", "Cindy", "Claire", "Clarence", "Cody", "Colby", "Connor", "Courtney", "Craig",
        "Daniel", "Danielle", "David", "Deborah", "Dennis", "Derek", "Devan", "Diana", "Diane", "Dominic", "Donald", "Donna",
        "Dylan", "Edward", "Eleanor", "Elijah", "Elizabeth", "Ella", "Emily", "Emma", "Eric", "Erin",
        "Ethan", "Evan", "Evelyn", "Faith", "Felix", "Florence", "Frank", "Gabriel", "George", "Grace", "Grant",
        "Gregory", "Hannah", "Harold", "Harry", "Hayden", "Heather", "Helly", "Henry", "Hunter", "Ian", "Isaac",
        "Isabella", "Jack", "Jacob", "James", "Jasmine", "Jason", "Jeffrey", "Jenna", "Jennifer", "Jeremy",
        "Jessica", "Joan", "Jon", "John", "Jonathan", "Jordan", "Joseph", "Joshua", "Julia", "Justin", "Karen",
        "Katherine", "Kathleen", "Katie", "Kayla", "Keith", "Kelly", "Kenneth", "Kevin", "Kimberly", "Korrey", "Kyle",
        "Laura", "Lauren", "Lawrence", "Leo", "Leonard", "Liam", "Lillian", "Lily", "Linda", "Logan",
        "Lucas", "Lucy", "Luis", "Luke", "Madeline", "Madison", "Margaret", "Maria", "Mark", "Mary",
        "Mason", "Mat", "Matt", "Megan", "Melanie", "Melissa", "Michael", "Michelle", "Molly", "Morgan", "Nancy",
        "Natalie", "Nathan", "Nicholas", "Nicole", "Noah", "Oliver", "Olivia", "Owen", "Pamela", "Patricia",
        "Patrick", "Paul", "Peter", "Philip", "Rachel", "Rebecca", "Reed", "Richard", "Ro", "Robert", "Roger", "Ronald",
        "Rose", "Ryan", "Sam", "Samantha", "Samuel", "Sandra", "Sarah", "Scott", "Sean", "Sebastian", "Seth", "Sharon",
        "Sophia", "Spencer", "Stephanie", "Stephen", "Steve", "Susan", "Sydney", "Taylor", "Teresa", "Thomas",
        "Timothy", "Travis", "Tyler", "Vanessa", "Victoria", "Vincent", "Walter", "Wayne", "William", "Zachary", "Zack"
    };
    private List<string> _availableNames;
    private readonly Random _random;

    public RandomNameGenerator()
    {
        _availableNames = new List<string>(_allNames);
        _random = new Random();
    }

    public string GetRandomName()
    {
        if (_availableNames.Count == 0)
        {
            ResetNames();
        }

        int index = _random.Next(_availableNames.Count);
        string firstName = _availableNames[index];
        _availableNames.RemoveAt(index);

        char lastInitial = (char)_random.Next('A', 'Z' + 1);

        return $"{firstName} {lastInitial}.";
    }

    private void ResetNames()
    {
        _availableNames = new List<string>(_allNames);
    }
}
