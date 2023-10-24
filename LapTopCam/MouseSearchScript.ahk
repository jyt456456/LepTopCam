global outx := 1
global outy := 2
global xstart :=1
global ystart :=1
global xend :=2
global yend :=2
global colorhex :=0055ff
global rang := 1



SetData(x,y,_xend,_yend,_rang, _color)
{
	xstart := x
	ystart := Y
	xend := _xend
	yend := _yend
	rang := _rang
	colorhex := _color
	
}

Init()
{
	CoordMode, Pixel, Screen
	CoordMode, Mouse, Screen
	MouseMove, 0,0 
}

RunSearch()
{
	PixelSearch ,px, py, %xstart%, %ystart%, %xend%, %yend%, %colorhex%, %rang%, Fast

	outx = %px%
	outy = %py%
	MsgBox 타겟위치 %outx%, %outy% ,컬러  %colorhex% , %xstart%, %xend%
	return ErrorLevel
	
}

MouseRun()
{
	MouseMove, %outx%, %outy%
	MsgBox 타겟 %outx%, %outy%
	MouseClick, left, %outx%, %outy%
}

MouseGet()
{
	MouseGetPos , MouseX, MouseY
	 PixelGetColor , color, %MouseX%, %MouseY%
	 MsgBox 현재커서위치ㅐ %color% , %MouseX%, %MouseY%
}


ImgSearch()
{
	CoordMode, Pixel, Screen
	CoordMode, Mouse, Screen

	ImageSearch, FoundX, FoundY, 0, 0, 2000, 2000, D:\mecro\load.png
	if(ErrorLevel =2)
		MsgBox 검색실행불가
	else if (ErrorLevel = 1)
		ImageSearch, FoundX, FoundY, 0, 0, 2000, 2000, D:\mecro\load2.png
	else
		real := 10
		realx := FoundX + real
		realy := FoundY + real
		MouseMove, %realx%, %realy%
		MouseClick, left, %realx%, %realy%
		Sleep, 600
		ImageSearch, FoundX, FoundY, 0,0, 2000, 2000, D:\mecro\filename.png
		MouseClick, left, %FoundX%, %FoundY%
		MouseClick, left, %FoundX%, %FoundY%

}

	