import csv
import os

os.chdir(dir)

with open("test.csv", 'rb') as csvfile:
	spamreader = csv.reader(csvfile, delimiter=',', quotechar='|')
	for row in spamreader:
		print ', '.join(row)