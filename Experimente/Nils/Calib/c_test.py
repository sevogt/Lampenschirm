
####
##
#mycdict = cv2.aruco.Dictionary_get(cv2.aruco.DICT_4X4_50)
#myboard = cv2.aruco.CharucoBoard_create(5, 7, 0.04, 0.02, mycdict)
#myboardimage = myboard.draw((2100,1500), None, 10, 1);
#cv2.imwrite("D:\myboard2.png", myboardimage)

import cv2
import numpy as np
from pprint import pprint

cap = cv2.VideoCapture(0 + cv2.CAP_DSHOW)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 3840)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 2160)
cap.set(cv2.CAP_PROP_ZOOM, 100)
cap.set(cv2.CAP_PROP_FOCUS, 0)
cap.set(cv2.CAP_PROP_AUTOFOCUS, 1)

my_dict = cv2.aruco.Dictionary_get(cv2.aruco.DICT_4X4_50)
my_board = cv2.aruco.CharucoBoard_create(5, 7, 3.8, 1.9, my_dict)


camera_matrix = np.load("camera_params.npy")
distortion_params = np.load("distortion_params.npy")

cv2.namedWindow("frame", cv2.WINDOW_NORMAL)
cv2.resizeWindow("frame",(1366,768))

while(True):
	# Capture frame-by-frame
	ret, frame = cap.read()
	frame_cp = frame.copy()
	
	# Display the resulting frame
	#cv2.imshow('frame',frame)
	
	marker_corners, marker_ids, _ = cv2.aruco.detectMarkers(frame, my_dict)
	
	if marker_ids is not None and marker_corners is not None:
		ret, charuco_corners, charuco_ids = cv2.aruco.interpolateCornersCharuco(marker_corners, marker_ids, frame, my_board)
		if charuco_corners is not None and charuco_ids is not None:
			frame_cp = cv2.aruco.drawDetectedCornersCharuco(frame_cp, charuco_corners, charuco_ids)

			ret, rvec, tvec = cv2.aruco.estimatePoseCharucoBoard(charuco_corners, charuco_ids, my_board, camera_matrix, distortion_params, None, None)

			if ret:

				R, _ = cv2.Rodrigues(rvec)

				plane_normal = R[2,:] # last row of plane rotation matrix is normal to plane
				plane_point = tvec.T

				#pprint("***")
				#pprint(plane_normal)
				#pprint(rvec)
				#pprint("***")
				pprint("###")
				pprint(plane_point)
				pprint("###")

				frame_cp = cv2.aruco.drawAxis(frame_cp, camera_matrix, distortion_params, rvec, tvec, 3.8)
	
	cv2.line(frame_cp, ((int)(frame_cp.shape[1]/2), 0), ((int)(frame_cp.shape[1]/2), (int)(frame_cp.shape[0])), (255, 0, 0), 6)
	cv2.line(frame_cp, (0,(int)(frame_cp.shape[0]/2)), ((int)(frame_cp.shape[1]), (int)(frame_cp.shape[0]/2)), (255,0,0), 6)
	cv2.imshow("frame",frame_cp)
	keycode = cv2.waitKey(1) & 0xFF
	if keycode == ord('q'):
		break

# When everything done, release the capture
cap.release()
#cv2.destroyAllWindows()
