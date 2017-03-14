
#include "cxcore.h"   
#include "cv.h"   
#include "highgui.h" 
#include "imgproc/imgproc.hpp"
#include "highgui/highgui.hpp"


using namespace cv;

// 内轮廓填充   
// 参数:   
// 1. pBinary: 输入二值图像，单通道，位深IPL_DEPTH_8U。  
// 2. dAreaThre: 面积阈值，当内轮廓面积小于等于dAreaThre时，进行填充。   
void FillInternalContours(IplImage *pBinary, double dAreaThre,IplImage *pBinary2)   
{   
	double dConArea;   
	CvSeq *pContour = NULL;   
	CvSeq *pConInner = NULL;   
	CvMemStorage *pStorage = NULL;   
	// 执行条件   
	if (pBinary)   
	{   
		// 查找所有轮廓   
		pStorage = cvCreateMemStorage(0);   
		cvFindContours(pBinary, pStorage, &pContour, sizeof(CvContour), CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE);   
		// 填充所有轮廓   
		if(fabs(cvContourArea(pContour, CV_WHOLE_SEQ))<200*200)
		{
		cvDrawContours(pBinary, pContour, CV_RGB(255, 255, 255), CV_RGB(255, 255, 255), 2, CV_FILLED, 8, cvPoint(0, 0));
		cvDrawContours(pBinary2, pContour, CV_RGB(255, 255, 255), CV_RGB(255, 255, 255), 2, CV_FILLED, 8, cvPoint(0, 0)); 
		}
		// 外轮廓循环   
		int wai = 0;  
		int nei = 0;  
		for (; pContour != NULL; pContour = pContour->h_next)   
		{   
			wai++;  
			// 内轮廓循环   
			//for (pConInner = pContour->v_next; pConInner != NULL; pConInner = pConInner->h_next)   
			//{   
			//	nei++;  
			//	// 内轮廓面积   
			//	dConArea = fabs(cvContourArea(pConInner, CV_WHOLE_SEQ));  
			//	printf("%f\n", dConArea);   
			//}  
			//CvRect rect = cvBoundingRect(pContour,0);
			//cvRectangle(pBinary, cvPoint(rect.x, rect.y), cvPoint(rect.x + rect.width, rect.y + rect.height),CV_RGB(255,255, 255), 1, 8, 0);
		}   

		//printf("wai = %d, nei = %d", wai, nei);  
		cvReleaseMemStorage(&pStorage);   
		pStorage = NULL;   
	}   
}   

void adaptiveThreshold(unsigned char* input, unsigned char*& bin, int width, int height)  
{  
    int S = width >> 3;  
    int T = 15;  
      
    unsigned long* integralImg = 0;  
    int i, j;  
    long sum=0;  
    int count=0;  
    int index;  
    int x1, y1, x2, y2;  
    int s2 = S/2;  
      
    bin = new unsigned char[width*height];  
    // create the integral image  
    integralImg = (unsigned long*)malloc(width*height*sizeof(unsigned long*));  
    for (i=0; i<width; i++)  
    {  
        // reset this column sum  
        sum = 0;  
        for (j=0; j<height; j++)  
        {  
            index = j*width+i;  
            sum += input[index];  
            if (i==0)  
                integralImg[index] = sum;  
            else  
                integralImg[index] = integralImg[index-1] + sum;  
        }  
    }  
    // perform thresholding  
    for (i=0; i<width; i++)  
    {  
        for (j=0; j<height; j++)  
        {  
            index = j*width+i;  
            // set the SxS region  
            x1=i-s2; x2=i+s2;  
            y1=j-s2; y2=j+s2;  
            // check the border  
            if (x1 < 0) x1 = 0;  
            if (x2 >= width) x2 = width-1;  
            if (y1 < 0) y1 = 0;  
            if (y2 >= height) y2 = height-1;  
            count = (x2-x1)*(y2-y1);  
            // I(x,y)=s(x2,y2)-s(x1,y2)-s(x2,y1)+s(x1,x1)  
            sum = integralImg[y2*width+x2] -  
                integralImg[y1*width+x2] -  
                integralImg[y2*width+x1] +  
                integralImg[y1*width+x1];  
            if ((long)(input[index]*count) < (long)(sum*(100-T)/100))  
                bin[index] = 0;  
            else  
                bin[index] = 255;  
        }  
    }  
    free (integralImg);  
} 
int Otsu(IplImage* src)      
{      
	int height=src->height;      
	int width=src->width;          

	//histogram      
	float histogram[256] = {0};      
	for(int i=0; i < height; i++)    
	{      
		unsigned char* p=(unsigned char*)src->imageData + src->widthStep * i;      
		for(int j = 0; j < width; j++)     
		{      
			histogram[*p++]++;      
		}      
	}      
	//normalize histogram      
	int size = height * width;      
	for(int i = 0; i < 256; i++)    
	{      
		histogram[i] = histogram[i] / size;      
	}      

	//average pixel value      
	float avgValue=0;      
	for(int i=0; i < 256; i++)    
	{      
		avgValue += i * histogram[i];  //整幅图像的平均灰度    
	}       

	int threshold;        
	float maxVariance=0;      
	float w = 0, u = 0;      
	for(int i = 0; i < 256; i++)     
	{      
		w += histogram[i];  //假设当前灰度i为阈值, 0~i 灰度的像素(假设像素值在此范围的像素叫做前景像素) 所占整幅图像的比例    
		u += i * histogram[i];  // 灰度i 之前的像素(0~i)的平均灰度值： 前景像素的平均灰度值    

		float t = avgValue * w - u;      
		float variance = t * t / (w * (1 - w) );      
		if(variance > maxVariance)     
		{      
			maxVariance = variance;      
			threshold = i;      
		}      
	}      

	return threshold;      
}     


void PrePreproduce(IplImage *img,Mat src)
{
	int height,width;
	int i,j,r,g,b,nh,nv;
    int flag1=0,flag2=0,tag=0;
	CvScalar c,c2;
	height=img->height;
	width=img->width;

	for(int i = 0;i < height;i++)
	{
        for(int j = 0;j < width;j++)
        {
		   c = cvGet2D(img,i,j); // 获取图像像素点的值
		   //c2 = cvGet2D(img2,i,j);
		   r=c.val[0];
		   g=c.val[1];
		   b=c.val[2]; 
		   
		   if(r>253&&g>253&&b>253)
		   { 
				src.at<Vec3b>(i,j)[0]=0;  
				src.at<Vec3b>(i,j)[1]=0;  
				src.at<Vec3b>(i,j)[2]=0;

		   }
        }
	}
}



void overturn(IplImage *img,IplImage *img2,Mat src)
{
	int height,width;
	int i,j,r,g,b,nh,nv;
    int flag1=0,flag2=0,tag=0;
	CvScalar c,c2;
	height=img->height;
	width=img->width;

	for(int i = 0;i < height;i++)
	{
        for(int j = 0;j < width;j++)
		{
			 c = cvGet2D(img2,i,j); // 获取图像像素点的值
		   //c2 = cvGet2D(img2,i,j);
		   r=c.val[0];
		   g=c.val[1];
		   b=c.val[2]; 
		   if(r!=255)
			   flag1++;
		   else
			   flag2++;

		}
	}

	if (flag2<8000)
	{	
		tag=1;

	}

	flag1=0;
	flag2=0;

	for(int i = 0;i < 10;i++)
	{
        for(int j = 0;j < 10;j++)
        {
		   c = cvGet2D(img2,i,j); // 获取图像像素点的值
		   //c2 = cvGet2D(img2,i,j);
		   r=c.val[0];
		   g=c.val[1];
		   b=c.val[2]; 
		   if(r!=255)
			   flag1++;
		   else
			   flag2++;

		}
	}



	if(tag==0)
	{
	for(int i = 0;i < height;i++)
	{
        for(int j = 0;j < width;j++)
        {
		   c = cvGet2D(img,i,j); // 获取图像像素点的值
		   //c2 = cvGet2D(img2,i,j);
		   r=c.val[0];
		   g=c.val[1];
		   b=c.val[2]; 
		   
		   if(flag1>flag2)
		   {
			   if(r==255&&g==255&&b==255)//不是白色的点（临时处理方法）
			  { 
					c.val[0]=c.val[0];

			   }
			   else
			   {
					/**cvSet2D(src,i,j,cvScalar(0,0,0));*/	
				    src.at<Vec3b>(i,j)[0]=0;  
                    src.at<Vec3b>(i,j)[1]=0;  
                    src.at<Vec3b>(i,j)[2]=0;  
				   
			   }
		   }
		   else
		   {
			    if(r==255&&g==255&&b==255)//不是白色的点（临时处理方法）
			  { 
					
				    src.at<Vec3b>(i,j)[0]=0;  
                    src.at<Vec3b>(i,j)[1]=0;  
                    src.at<Vec3b>(i,j)[2]=0;  

			   }
			   else
			   {
						  c.val[0]=c.val[0];
			   }
		   }
        }
	}
	}
}
void ImageBinarization(IplImage *src)
{	/*对灰度图像二值化，自适应门限threshold*/
	int i,j,width,height,step,chanel,threshold;
	/*size是图像尺寸，svg是灰度直方图均值，va是方差*/
	float size,avg,va,maxVa,p,a,s;
	unsigned char *dataSrc;
	float histogram[256];
	
	width = src->width;
	height = src->height;
	dataSrc = (unsigned char *)src->imageData;
	step = src->widthStep/sizeof(char);
	chanel = src->nChannels;
	/*计算直方图并归一化histogram*/
	for(i=0; i<256; i++)
		histogram[i] = 0;
	for(i=0; i<height; i++)
		for(j=0; j<width*chanel; j++)
		{
			histogram[dataSrc[i*step+j]-'0'+48]++;
		}
	size = width * height;
	for(i=0; i<256; i++)
		histogram[i] /=size;
	/*计算灰度直方图中值和方差*/
	avg = 0;
	for(i=0; i<256; i++)
		avg += i*histogram[i];
	va = 0;
	for(i=0; i<256; i++)
		va += fabs(i*i*histogram[i]-avg*avg);
	/*利用加权最大方差求门限*/
	threshold = 20;
	maxVa = 0;
	p = a = s = 0;
	for(i=0; i<256; i++)
	{
		p += histogram[i];
		a += i*histogram[i];
		s = (avg*p-a)*(avg*p-a)/p/(1-p);
		if(s > maxVa)
		{
			threshold = i;
			maxVa = s;
		}
	}
	/*二值化*/
	for(i=0; i<height; i++)
		for(j=0; j<width*chanel; j++)
		{
			if(dataSrc[i*step+j] > threshold)
				dataSrc[i*step+j] = 255;
			else
				dataSrc[i*step+j] = 0;
		}
}

void PreProduce(char *name,char *namedst)  
{  

	//IplImage *i = cvLoadImage(name, 1); 
	//Mat dst=imread(name, 1); ;
	//PrePreproduce(i,dst);
	// imwrite(namedst,dst);

	IplImage *img = cvLoadImage(name,0);  
	IplImage *img2 = cvLoadImage(name, 1); 
	Mat src = imread(name, 1); 
	IplImage *bin = cvCreateImage(cvGetSize(img), 8, 1);  

	int thresh = Otsu(img); 
	//ImageBinarization(img);
	//thresh=200;
	//cvThreshold(img, bin, thresh, 255, CV_THRESH_BINARY);  
	//cvAdaptiveThreshold(img, bin, thresh, 255, CV_THRESH_BINARY,25,10); 
	cvAdaptiveThreshold(img, bin, thresh, CV_ADAPTIVE_THRESH_MEAN_C, CV_THRESH_BINARY, 151, 7);  
	
	


	FillInternalContours(bin, 200,img2);  

	overturn(img2,bin,src);

	// //imshow("da",src);

    imwrite(namedst,src);


	//cvNamedWindow("img");  
	//cvShowImage("img", img);  

	//cvNamedWindow("img2");  
	//cvShowImage("img2", img2);

	//cvNamedWindow("result");  
	//cvShowImage("result", bin);  

	cvWaitKey(-1);  

	
	cvReleaseImage(&img);  
	cvReleaseImage(&bin);  
}  