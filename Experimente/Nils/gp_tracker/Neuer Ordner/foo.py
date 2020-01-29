import cv2

# capture = cv2.VideoCapture(0 + cv2.CAP_DSHOW)
'''capture = cv2.VideoCapture("D:\\gp_tracker\\Neuer Ordner\\test.avi")

capture.set(cv2.CAP_PROP_FRAME_WIDTH, 3840)
capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 2160)
fps = capture.get(cv2.CAP_PROP_FPS)
fw=3840
fh=2160'''
fw=3840
fh=2160
# out = cv2.VideoWriter('test.avi',cv2.VideoWriter_fourcc('M','J','P','G'), 30, (fw,fh))
out = cv2.VideoWriter('test.avi',cv2.VideoWriter_fourcc('M','J','P','G'), 30, (fw,fh))

for i in range(10):
    capture = cv2.VideoCapture("D:\\gp_tracker\\Neuer Ordner\\test_original.avi")

    capture.set(cv2.CAP_PROP_FRAME_WIDTH, 3840)
    capture.set(cv2.CAP_PROP_FRAME_HEIGHT, 2160)
    fps = capture.get(cv2.CAP_PROP_FPS)


    while True:
        ret, frame = capture.read()
        if frame is None:
            break
        out.write(frame)

        cv2.imshow("foo", frame)
        keyboard = cv2.waitKey(1)
        if keyboard == ord('q'):
            break

    capture.release()

# while True:
#     ret, frame = capture.read()
#     if frame is None:
#         break
#     out.write(frame)

#     cv2.imshow("foo", frame)
#     keyboard = cv2.waitKey(1)
#     if keyboard == ord('q'):
#         break

# cap.release()
out.release()
 
cv2.destroyAllWindows() 