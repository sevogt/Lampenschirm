
import cv2
import numpy as np
from pprint import pprint

## cameraMatrix ...
## distCoeffs

#cap = cv2.VideoCapture(1)

calib_images=[]
for i in range(0,20):
	img = cv2.imread("logi_1_frame_"+str(i)+".png", cv2.IMREAD_COLOR)
	calib_images.append(img)
	
unit=3.9 #cm
object_points = np.zeros((6*4,3),dtype=np.float32)
for i in range(6):
	for j in range(4):
		object_points[i*4+j,0]=j*unit
		object_points[i*4+j,1]=i*unit
#pprint(object_points)

my_dict = cv2.aruco.Dictionary_get(cv2.aruco.DICT_4X4_50)
my_board = cv2.aruco.CharucoBoard_create(5, 7, 3.8, 1.9, my_dict)

####
charucoCornersAccum = []
charucoIdsAccum = []
i=0
j=0
for img in calib_images:
	pprint("a"+str(i))
	i+=1
	corners, ids, rejected = cv2.aruco.detectMarkers(img, my_dict)

	corners, ids, rejected, recovered = cv2.aruco.refineDetectedMarkers(img, my_board, corners, ids, rejected)
 
	ret, charucoCorners, charucoIds = cv2.aruco.interpolateCornersCharuco(corners, ids, img, my_board)
	#pprint(charucoIds)
	pprint(len(charucoIds))
	pprint(len(charucoCorners))
	if( len(charucoIds) < 4 ):
		j+=1
		pprint("YYYY")
		continue
	charucoCornersAccum += [charucoCorners]
	charucoIdsAccum += [charucoIds]
	
pprint("XXX: "+str(j))
print("calibrate camera")
# calibrate camera
ret, K, dist_coef, rvecs, tvecs = cv2.aruco.calibrateCameraCharuco(charucoCornersAccum,
																	charucoIdsAccum,
																	my_board,
																	(1920, 1080),
																	None,
																	None)
pprint(K)
pprint("***")
pprint(dist_coef)
####
camera_matrix = np.load("camera_params.npy")
distortion_params = np.load("distortion_params.npy")
pprint("***")
pprint(camera_matrix)
pprint(distortion_params)

#np.save("camera_params.npy", K)
#np.save("distortion_params.npy", dist_coef)

exit(1)
i=1
detected_corners_arr = []
object_points_arr = []
for img in calib_images:
	
	ret, my_corners = cv2.findChessboardCorners(img,(4,6), flags=cv2.CALIB_CB_ADAPTIVE_THRESH | cv2.CALIB_CB_NORMALIZE_IMAGE)

	if not ret:
		print("Error file "+str(i))
		continue

	timg = img.copy()
	img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
	cv2.cornerSubPix(img_gray, my_corners, (11, 11), (-1, -1), (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_COUNT, 30, 0.1))
	
	timg = cv2.drawChessboardCorners(timg, (4,6), my_corners, ret)
	cv2.imshow("cur", timg)
	cv2.waitKey(0)

	detected_corners_arr.append(my_corners)
	object_points_arr.append(object_points)
	

	i+=1

cv2.destroyAllWindows()

ret, camera_params, distortion_params, rvecs, tvecs = cv2.calibrateCamera(object_points_arr, detected_corners_arr, (1920, 1080), None,None)
pprint("###")
pprint(rvecs)
pprint("###")
pprint(tvecs)
pprint("+++")
np.save("camera_params.npy", camera_params)
np.save("distortion_params.npy", distortion_params)

total_error = 0
for i in range(len(object_points_arr)):
	transformed_img_points, _ = cv2.projectPoints(object_points_arr[i], rvecs[i], tvecs[i], camera_params, distortion_params)
	#pprint(transformed_img_points)
	error = cv2.norm(detected_corners_arr[i],transformed_img_points, cv2.NORM_L2)/len(transformed_img_points)
	print("e:"+str(error))
	total_error += error

print("total error: "+str(total_error/len(object_points_arr)))

#ret, camera_matrix, dist_coeffs, rvecs, tvecs, = cv2.calibrateCamera()
## Size	  Square size in m
## (4,6), 0.039



#object_points = np.array(object_points)
#object_points_arr = [np.array([object_points]), *object_points, *object_points, *object_points, *object_points, *object_points, *object_points]
#object_points_arr = np.array(object_points_arr)


pprint("***")

#ret, camera_matrix, dist_coeffs, rvecs, tvecs, = cv2.calibrateCamera(object_points_arr, detected_corners, (4,6), None, None)
