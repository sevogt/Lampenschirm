
import cv2
import numpy as np
from pprint import pprint

cap = cv2.VideoCapture(2 + cv2.CAP_DSHOW)
v_width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
v_height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
v_fps = cap.get(cv2.CAP_PROP_FPS)
v_zoom = cap.get(cv2.CAP_PROP_ZOOM)
v_focus = cap.get(cv2.CAP_PROP_FOCUS)
v_iso = cap.get(cv2.CAP_PROP_ISO_SPEED)
v_autof = cap.get(cv2.CAP_PROP_AUTOFOCUS)

pprint((v_width, v_height, v_fps, v_zoom, v_focus, v_iso, v_autof))
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 3840)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 2160)
cap.set(cv2.CAP_PROP_ZOOM, 100)
cap.set(cv2.CAP_PROP_FOCUS, 0)
cap.set(cv2.CAP_PROP_AUTOFOCUS, 1)

v_width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
v_height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
v_zoom = cap.get(cv2.CAP_PROP_ZOOM)
v_focus = cap.get(cv2.CAP_PROP_FOCUS)
pprint((v_width, v_height, v_fps, v_zoom, v_focus, v_iso, v_autof))

filectr=0

cv2.namedWindow("frame", cv2.WINDOW_NORMAL)
cv2.resizeWindow("frame",(1366,768))


my_dict = cv2.aruco.Dictionary_get(cv2.aruco.DICT_4X4_50)
my_board = cv2.aruco.CharucoBoard_create(5, 7, 3.8, 1.9, my_dict)


while(True):

    for i in range(5):
        ret, frame = cap.read()
        cv2.imshow("frame",frame)
        cv2.waitKey(1)

    marker_corners, marker_ids, _ = cv2.aruco.detectMarkers(frame, my_dict)

    if marker_ids is not None and marker_corners is not None:
        ret, charuco_corners, charuco_ids = cv2.aruco.interpolateCornersCharuco(marker_corners, marker_ids, frame, my_board)
        frame_cp = frame.copy()
        frame_cp = cv2.aruco.drawDetectedCornersCharuco(frame_cp, charuco_corners, charuco_ids)
        cv2.imshow("frame",frame_cp)
        if charuco_corners is not None and charuco_ids is not None:
            
            keycode = cv2.waitKey(0) & 0xFF
            if keycode == ord('x'):
                continue
            cv2.imwrite("D:\logi_1_frame_"+str(filectr)+".png", frame)
            filectr+=1
            print("Image saved " + str(filectr))

    keycode = cv2.waitKey(2) & 0xFF
    if keycode == ord('q'):
        break


cap.release()
cv2.destroyAllWindows()
