#!/usr/bin/env python3
import uuid, sys

class UUIDs():

    def print_uuids_to_file(self, count, out):
        num = int(count)
        with open(out, 'w') as file:
            for index in range(num):
                file.write("{0}: {1}\n".format(index+1, uuid.uuid1()))

def main(argv):
    uuids = UUIDs()
    uuids.print_uuids_to_file(argv[0], argv[1])

if  __name__ =='__main__':main(sys.argv[1:])
