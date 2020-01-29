
import cv2
import numpy as np
from pprint import pprint

cap = cv2.VideoCapture(1 + cv2.CAP_DSHOW)

v_width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
v_height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
v_fps = cap.get(cv2.CAP_PROP_FPS)
v_zoom = cap.get(cv2.CAP_PROP_ZOOM)
v_focus = cap.get(cv2.CAP_PROP_FOCUS)
v_iso = cap.get(cv2.CAP_PROP_ISO_SPEED)
v_autof = cap.get(cv2.CAP_PROP_AUTOFOCUS)

#pprint((v_width, v_height, v_fps, v_zoom, v_focus, v_iso, v_autof))
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1920)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1080)
v_width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
v_height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
pprint((v_width, v_height, v_fps, v_zoom, v_focus, v_iso, v_autof))

filectr=0

cv2.namedWindow("frame", cv2.WINDOW_NORMAL)
cv2.resizeWindow("frame",(1366,768))

while(True):
	
	for i in range(30):
		ret, frame = cap.read()
		#cv2.namedWindow("frame", cv2.WINDOW_NORMAL)
		#cv2.resizeWindow("frame",(1366,768))
		cv2.imshow("frame",frame)
		cv2.waitKey(1)

	#pprint(frame.shape)
	ret, my_corners = cv2.findChessboardCorners(frame,(4,6), flags=cv2.CALIB_CB_ADAPTIVE_THRESH |+ cv2.CALIB_CB_NORMALIZE_IMAGE)
	#print(my_corners)
	
	timg = frame.copy()
	timg = cv2.drawChessboardCorners(timg, (4,6), my_corners, ret)
	#cv2.namedWindow("frame", cv2.WINDOW_NORMAL)
	#cv2.resizeWindow("frame",(1366,768))
	cv2.imshow("frame",timg)
	
	keycode = cv2.waitKey(0) & 0xFF
	if keycode == ord('q'):
		break
	elif keycode == ord('s'):
		filectr += 1
		cv2.imwrite("D:\calib3_frame_"+str(filectr)+".png", frame)
		print("Image saved")
	
	
cap.release()
cv2.destroyAllWindows()
