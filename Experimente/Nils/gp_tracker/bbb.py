
import cv2
import numpy as np
from pprint import pprint

def detectAndDisplay(frame):
	frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
	frame_gray = cv2.equalizeHist(frame_gray)

	#pointers = ptr_cc.detectMultiScale(frame_gray)
	#for (x,y,w,h) in pointers:
#		center = (x + w//2, y + h//2)
#		frame = cv2.ellipse(frame, center, (w//2, h//2), 0, 0, 360, (255, 0, 255), 4)
	cv2.imshow('frame', frame)


ptr_cascade_filename = "hand.xml" #"hff.xml"
ptr_cc = cv2.CascadeClassifier()

if not ptr_cc.load(ptr_cascade_filename):
#if not ptr_cc.load(ptr_cascade_filename):
	print(".xml nicht gefunden")
	exit(1)

cap = cv2.VideoCapture(0 + cv2.CAP_DSHOW)

if not cap.isOpened:
    print("Konnte Kamera nicht oeffnen")
    exit(1)

while True:
	ret, frame = cap.read()
	if frame is None:
		print('Konnte keinen Frame aquirieren')
		break
    #detectAndDisplay(frame)
	frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
	frame_gray = cv2.equalizeHist(frame_gray)

	pointers = ptr_cc.detectMultiScale(frame_gray)
	for (x,y,w,h) in pointers:
		center = (x + w//2, y + h//2)
		frame = cv2.ellipse(frame, center, (w//2, h//2), 0, 0, 360, (0, 0, 255), 4)
	cv2.imshow('frame', frame)

	if cv2.waitKey(1) == ord('q'):
		break

cv2.destroyAllWindows()
