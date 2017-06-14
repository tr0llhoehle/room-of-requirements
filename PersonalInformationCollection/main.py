"""
"""

from ApplyPersonalityChanges import ApplyPersonalityChanges
from NextVideos import NextVideos
from Personality import Personality
# import VideoControlInformation or open some channel to it
# This means that there is new information, and the viewed video is set
VideoControlInformation = True
viewed_video = "videoA81"

# This should be a json containing that information
video_traits = {"videoA81": {"extraversion": {"plus": 5, "times": 1.2, "absolute": 0},
                             "neuroticism": {"plus": 0, "times": 1, "absolute": 60}},
                "videoB2": {"agreeableness": {"plus": 3, "times": 1, "absolute": 0},
                            "neuroticism": {"plus": -10, "times": 1, "absolute": 0}}}

if __name__ == '__main__':
    personality = Personality()
    # print("Personality is in the beginning: ", personality.dict_all_traits, personality.changes_made)
    if VideoControlInformation is True:
        apc = ApplyPersonalityChanges(personality, video_traits, viewed_video)
        print("Personality is now: ", personality.dict_all_traits, "\nChanges made are: ", personality.changes_made)
        nv = NextVideos(personality, video_traits)
        ratings = nv.video_rater()
        print("Ratings", ratings)
        recommendations = nv.video_chooser(ratings)
        print("Recommendations", recommendations)
