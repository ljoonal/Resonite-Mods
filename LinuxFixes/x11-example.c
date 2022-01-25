#include <X11/Xlib.h>
#include <stdio.h>

int main() {
  Display *dpy = XOpenDisplay(NULL);
  Window window;
  int revert_to_return;
  XGetInputFocus(dpy, &window, &revert_to_return);
  XWarpPointer(dpy, None, window, None, None, None, None, 100, 100);
  XFlush(dpy);
}
