"""
Singleton class containing all known personality traits/attributes.
"""


class Personality(object):
    def __init__(self):
        """
        Describes the five traits of the big five personality model.
        A value of 0 denotes e.g. a very introverted person, a value of 100 e.g. a very extroverted person.
        """
        self.openness = 0
        self.conscientiousness = 0
        self.extraversion = 0
        self.agreeableness = 0
        self.neuroticism = 0
        self.dict_all_traits = {"openness": self.openness, "conscientiousness": self.conscientiousness,
                                "extraversion": self.extraversion, "agreeableness": self.agreeableness,
                                "neuroticism": self.neuroticism}

        # Saves the amount of changes/updates that were made to a trait.
        # This is a measure for how sure we are of that trait's prediction.
        self.changes_made = {"openness": 0, "conscientiousness": 0, "extraversion": 0, "agreeableness": 0,
                             "neuroticism": 0}

    def update_dict(self):
        self.dict_all_traits = {"openness": self.openness, "conscientiousness": self.conscientiousness,
                                "extraversion": self.extraversion, "agreeableness": self.agreeableness,
                                "neuroticism": self.neuroticism}

