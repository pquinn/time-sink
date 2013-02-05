__author__ = 'phil'

#a jump data point is essentially just a time when the player jumped
class JumpDataPoint():
    def __init__(self, time):
        self.time = time

    def __str__(self):
        return "Player jumped at time {time}".format(time=self.time)

#a health data point is a player's health value at a time snapshot
class PlayerSnapshotDataPoint():
    """Holds relevant data for player snapshots, currently set to log every 1 second.

    >>> point1 = PlayerSnapshotDataPoint(100, 0, "{X:100, Y: 100)"

    """
    def __init__(self, time, health, mana, position):
        self.time = time
        self.health = health
        self.mana = mana
        self.position = position

    def get_health(self):
        return self.health

    def get_time(self):
        return self.time

    def get_mana(self):
        return self.mana

    def __str__(self):
        return "Data Point: {time}: health = {health}, mana = {mana}, position = {position}".format(time = self.time, health=self.health, mana=self.mana, position=self.position)

#when a player dies
class DeathDataPoint():
    def __init__(self, location, time):
        self.location = location
        self.time = time

    def __str__(self):
        return "Player died at {location} at time {time}".format(position=self.location, time=self.time)



  