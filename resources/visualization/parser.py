__author__ = 'phil'
from data_points import *
from datetime import datetime, timedelta
from time import strptime, mktime
import collections
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.mlab as mlab

def parse_lines(file_path):
    lines = []
    with open(file_path) as fd:
        for line in fd:
            lines.append(list(line.split()))
    return lines

def format_time(time_str):
    #http://docs.python.org/2/library/time.html
    #year-month-day hour:minute:second,milli
    #%Y-%m-%d %H:%M:%S,%f
    formatted_time = strptime(time_str, "%Y-%m-%d %H:%M:%S,%f")
    dt = datetime.fromtimestamp(mktime(formatted_time))
    return dt

def timestamp(dt):
    return mktime(dt.timetuple())

#def get_time_in_seconds(time_struct):

def get_time_span(times):
    return max(times) - min(times)

def time_to_str(time_in):
    return ""

def create_data_points(lines):
    #for each line, find out what type of data point it is, then make it
    created_data_points = []
    for index, line in enumerate(lines):
        if "health:" in line:
            #this is so fucking ugly
            time = format_time("{date} {time}".format(date=line[0], time=line[1]))
            heatlh = line[9]
            mana = lines[index+1][9]
            position = "{x} {y}".format(x=lines[index+2][9], y=lines[index+2][10])
            created_data_points.append(PlayerSnapshotDataPoint(time, heatlh, mana, position))
    return created_data_points

def print_lines(lines):
    for line in lines:
        print "line: {line}".format(line=line)

def print_data_points(points):
    for point in points:
        print str(point)

def init_graph(times):
    plt.xlabel('Time')
    plt.ylabel('Player Health')
    plt.title('Player Health over Time')
    plt.xlim(0, get_time_span(times))
    plt.ylim(0, 100)

def graph_snapshots(snapshots):
    times = get_times_seconds(get_times(snapshots))
    print "len of times: {len}".format(len=len(times))
    health_values = get_health_values(snapshots)
    print "len of healths: {len}".format(len=len(health_values))
    init_graph(times)
    
    plt.plot(times, health_values)
    plt.show()

def get_times(snapshots):
    values = []
    for snapshot in snapshots:
        values.append(snapshot.get_time())

    return values

def get_times_seconds(times):
    values = []
    first_time = times[0]
    for time_value in times:
        delta_seconds = timestamp(time_value) - timestamp(first_time)
        values.append(delta_seconds)

    return values

def get_health_values(snapshots):
    values = []
    for snapshot in snapshots:
        values.append(snapshot.get_health())

    return values
if __name__ == '__main__':
    import sys
    file_path = sys.argv[1]
    #print_lines(tokenize_lines(parse_lines(file_path)))
    #tokenize_lines(parse_lines(file_path))
    #print_lines(parse_lines(file_path))
    #print_data_points(create_data_points(parse_lines(file_path)))
    #print(type(parse_lines(file_path)))
    graph_snapshots(create_data_points(parse_lines(file_path)))