"""
This is a (fake) class for communication with Video Control.
It takes the information about viewed videos and updates/changes personality trait information.
"""


class ApplyPersonalityChanges(object):
    def __init__(self, personality, video_traits, viewed_video):
        """
        viewed_video is the video that was predominantly watched by the visitor.
        """
        self.personality = personality
        self.video_traits = video_traits
        self.viewed_video = viewed_video
        self.change_values()

    def json_lookup(self, viewed_video):
        """
        Look up the video's influences on personality traits. E.g. {"videoA81": {"extraversion":
        {"plus": 5, "times": 1.2, "absolute": 0}, "neuroticism": {"plus": 0, "times": 1, "absolute": 60}}}
        Plus is a value that should be added to the previous trait information.
        Times is a value that the previous trait information should be multiplied with.
        Absolute is the absolute value that the trait information should be set to.
        :param viewed_video: The video that was watched
        :return: All change values
        """
        return self.video_traits[viewed_video]

    def apply_personality_values(self, traits_to_change):
        """
        Apply the changes on personality traits to the already collected personality traits.
        :param traits_to_change: All of the changes, e.g. {"extraversion": 5, "neuroticism": -10}
        """
        for trait in traits_to_change:
            self.personality.changes_made[trait] += 1
            if trait == "openness":
                self.personality.openness += traits_to_change[trait]["plus"]
                self.personality.openness *= traits_to_change[trait]["times"]
                if traits_to_change[trait]["absolute"] != 0:
                    self.personality.openness = traits_to_change[trait]["absolute"]
            if trait == "conscientiousness":
                self.personality.conscientiousness += traits_to_change[trait]["plus"]
                self.personality.conscientiousness *= traits_to_change[trait]["times"]
                if traits_to_change[trait]["absolute"] != 0:
                    self.personality.conscientiousness = traits_to_change[trait]["absolute"]
            if trait == "extraversion":
                self.personality.extraversion += traits_to_change[trait]["plus"]
                self.personality.extraversion *= traits_to_change[trait]["times"]
                if traits_to_change[trait]["absolute"] != 0:
                    self.personality.extraversion = traits_to_change[trait]["absolute"]
            if trait == "agreeableness":
                self.personality.agreeableness += traits_to_change[trait]["plus"]
                self.personality.agreeableness *= traits_to_change[trait]["times"]
                if traits_to_change[trait]["absolute"] != 0:
                    self.personality.agreeableness = traits_to_change[trait]["absolute"]
            if trait == "neuroticism":
                self.personality.neuroticism += traits_to_change[trait]["plus"]
                self.personality.neuroticism *= traits_to_change[trait]["times"]
                if traits_to_change[trait]["absolute"] != 0:
                    self.personality.neuroticism = traits_to_change[trait]["absolute"]

    def change_values(self):
        values_to_change = self.json_lookup(self.viewed_video)
        self.apply_personality_values(values_to_change)
        self.personality.update_dict()
