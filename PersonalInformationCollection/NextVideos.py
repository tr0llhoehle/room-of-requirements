"""
Maximizes known personality traits by choosing best next videos to show.
"""

import operator

# Video list is a list of all videos that we have (probably saved in a json/txt file/whatever)
video_list = ["videoA81", "videoB2"]

# This is a dict counting how often we showed a video so far
times_shown = {"videoA81": 0, "videoB2": 0}

# These ratings are used to reward/punish videos
SMALL_RATING = 2
MIDDLE_RATING = 5
BIG_RATING = 10


class NextVideos(object):
    def __init__(self, personality, video_traits):
        """
        video_ratings: A dict assigning every video a rating which denotes how much information we gain from it, i.e.
        a higher rating means we should show this video now, a lower rating means we shouldn't show it now.
        """
        self.personality = personality
        self.video_traits = video_traits

    # Now find videos that give us information about the traits we know least about.
    # Rate them according to the amount of information gain that we get from them.
    # Higher rating: More information gain. Lower rating: Less information gain.

    def video_rater(self):
        # video_ratings: A dict assigning every video a rating which denotes how much information we gain from it, i.e.
        # a higher rating means we should show this video now, a lower rating means we shouldn't show it now.
        video_ratings = {video: 0 for video in video_list}
        # This says how often every trait has had a change so far
        # {"openness": 0, "conscientiousness": 2, "extraversion": 1, "agreeableness": 4, "neuroticism": 3}
        changes_made = self.personality.changes_made
        # Sorted by values: [('openness', 0), ('extraversion', 1),
        #                    ('conscientiousness', 2), ('neuroticism', 3), ('agreeableness', 4)]
        changes_sorted = sorted(changes_made.items(), key=operator.itemgetter(1))
        # As a list of the ordered trait names: ['openness', 'extraversion',
        #                                        'conscientiousness', 'neuroticism', 'agreeableness']
        changes_sorted_list = [trait[0] for trait in changes_sorted]
        # Same list, but highest values first: ['agreeableness', 'neuroticism',
        #                                       'conscientiousness', 'extraversion', 'openness']
        changes_sorted_list_reverse = list(reversed(changes_sorted_list))
        for video in video_list:
            # Rate videos lower that have already been shown
            # TODO: Update with actual times seen, currently will always be 0
            video_ratings[video] -= MIDDLE_RATING * times_shown[video]
            trait_changes = self.video_traits[video]
            for trait in trait_changes:
                # Rate the video higher if the trait was updated only a few times so far
                video_ratings[video] += SMALL_RATING * changes_sorted_list_reverse.index(trait)
                # If the trait was updated not at all or only once, favor absolute changes
                if "absolute" in trait_changes and changes_made[trait] in [0, 1]:
                    video_ratings[video] += BIG_RATING
                # The lower a number (< 50), the more favor "plus" ratings, assuming that most people fall in the middle
                # therefore such low ratings are unlikely
                if self.personality.dict_all_traits[trait] < 50 and trait_changes[trait]["plus"] != 0:
                    video_ratings[video] += SMALL_RATING
                # The farther a rating to the middle (35 < rating < 65), the more favor "multiplication"s ratings
                if 35 < self.personality.dict_all_traits[trait] < 65 and trait_changes[trait]["times"] != 1:
                    video_ratings[video] += SMALL_RATING
        return video_ratings

    def video_chooser(self, video_ratings):
        return [video[0] for video in list(reversed(sorted(video_ratings.items(), key=operator.itemgetter(1))))][:2]