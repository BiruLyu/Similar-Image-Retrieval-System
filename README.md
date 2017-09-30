# Similar-Image-Retrieval-System
This is a repository for **Similar Image Search**. It implemented search by image based on the color histograms, which have the following parts:

+ Retrieval Algorithm Design
  + [Background Removal](#background-removal)
  + [Generating color histogram](#generating-color-histogram)
  + [Color Distance](#color-distance)
  + [Ranking](#ranking)
  + [Conclusion](#conclusion)
+ [User Interface Design](#user-interface-design)

# Retrieval Algorithm Design

## <a name = "background-removal"></a>Background Removal

+ Determine the binarization threshold

  using [**Otsu's method**](https://www.wikiwand.com/en/Otsu%27s_method) to calculate the optimum threshold separating foreground and background.

+ Thresholding
  Based on the resulting threshold, use the functions provided by OpenCV for local adaptive thresholding.

+ Remove backgroud

  Find all contours for the image and fill all connected fields with less than 200x200 pixes in white. Since the main entities in the data set are centered.

  Take the 30 * 30 pixes rectangular area in the upper left corner of the image, and calculate the ratio of back and white pixels in this area. The color with larger proportion will be selected as background color.

  Apply this mask to the original image so as to remove the backgroud of the original image.

## <a name = "generating-color-histogram"></a>Generating Color Histogram

Using **hue-saturation-value (HSV)** color model instead of **RGB** color space. 

> using only the Hue component makes the algorithm less sensitive (if not invariant) to lighting variations.
>
> [Link](https://dsp.stackexchange.com/questions/2687/why-do-we-use-the-hsv-colour-space-so-often-in-vision-and-image-processing)

[Converting from RGB to HSV](http://coecsl.ece.illinois.edu/ge423/spring05/group8/finalproject/hsv_writeup.pdf)

![](https://ws2.sinaimg.cn/large/006tNc79gy1fk2b6kqy53j30hs06y3z8.jpg)

## <a name = "color-distance"></a>Color Distance

As the image background has been removed, so the histogram of the statistical results is the main color of the object in the picture, thus, a two image color matching results can be used as a basis for the two images similarity.
**Color distance** refers to the difference between the two colors. The color distance should reflect the color in the visual gap, it is the basic basis for image segmentation. However, in the **HSV color space**, the color information is only related to the `H` component and the `S`component. The color values in the `H` component are not linear, but rather a ring, and the perception of the color is also related to the `S `component.
[**Androutsos**](http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.94.2580&rep=rep1&type=pdf) have roughly divided the HSV color space by experiment, 

+ brightness greater than 75% and saturation greater than 20% are bright color areas 
+ brightness less than 25% is black area
+ brightness greater than 75% and saturation less than 20% is white areas, the other are the color areas

Handling **Black** and **white**  as special color in the HSV space.

+ Record the the mode of `Ｓ` component histogram of an image as variable `flag`

+  Record the mode of `V` component histogram of an image as variable `mark`

+ Record the mode of `H`  component histogram of a picture as `t` , and define variable `tag` as the hue of an image. Here's how to evaluate it:

  ```cpp
  if(t >= 10 && t < 115){
      every[n].tag = (int)(t*36/360);
  } else if (t >= 115 && t < 185) {
      every[n].tag = 12;
  } else if (t >= 185 && t < 195) {
      every[n].tag = 14;
  } else if (t >= 195 && t < 219) {
      every[n].tag = 14;
  } else if (t >= 219 && t < 230) {
      every[n].tag = 16;
  } else if (t >= 230 && t < 240) {
      every[n].tag = 18;
  } else if (t >= 240 && t < 260) {
      every[n].tag = 19;
  } else {
      every[n].tag = 0;
  }
  ```

Finally, define the color distance between two points as follow :  
![](https://ws4.sinaimg.cn/large/006tNc79gy1fk2dyr7gesj319y0mwn14.jpg)

<script>
$$ a_i=\left\{\begin{array}{rcl}0       &      & {v_i      <      25\%}\\2     &      & {v_i > 75\% \ and \ s_i < 20\%}\\1     &      & {other}\\\end{array}\right. $$ 

$$ h_{ij}=\left \{ \begin{array}{rcl}|tag_i - tag_j| * |h_i - h_j|       &    & {|tag_i - tag_j|      <      10 \ and \ |h_i - h_j| < 180}\\(20 - |tag_i - tag_j|) * |h_i - h_j|     &      & {|tag_i - tag_j| > 10 \ and \ |h_i - h_j| < 180}\\|tag_i - tag_j| * (360 -  |h_i - h_j|)    &      & {|tag_i - tag_j| < 10 \ and \ |h_i - h_j| > 180}\\(20 - |tag_i - tag_j|) * (360 - |h_i - h_j|)       &    & {|tag_i - tag_j|      >      10 \ and \ |h_i - h_j| > 180}\\\end{array}\right. $$ 

$$s_i = |flag_i - flag_j|$$

$$ D_{ij}=\left \{ \begin{array}{rcl}10.2 * |mark_i - mark_j| * h_{ij}       &    & {a_i = 0 \ and \ a_j = 0}\\ |mark_i - mark_j|     &      & {a_i = 2 \ and \ a_j = 2}\\16.2 * h_{ij}  +1.9 * s_{ij} + 300 * |a_i - a_j|     &      & {|a_i - a_j| = 2 }\\0.22 * h_{ij}  + s_{ij} + 2.2 * |mark_i - mark_j|       &    & {a_i  = 0 \ or \ a_j = 0}\\0.22 * h_{ij}  + s_{ij} + |mark_i - mark_j|       &    & {a_i  = 2 \ or \ a_j = 2}\\ h_{ij}  + s_{ij} + |mark_i - mark_j| && {a_i = 1 \ and \ a_j = 1}\end{array}\right. $$ 
</script>



In the above formula, 

+  $a_i$ is a logical variable. 
  + When $a_i == 0$, it means that point `i` is black
  + when $a_i == 2$, it means that point `i `is white
  + In other cases, $a_i == 1$ 
+ $h_{ij}$ is the **hue** distance between points `i` and `j`
+ $s_{ij}$ is the **saturation** distance of the points  `i` and `j`
+ $mark_i $ and $mark_j$ respectively represent the brightness of points` i` and `j`
  The distance between the **black** and **white** points has a lot to do with the brightness, but it it less relevant to  the hue and saturation. 
+ `10.2` and `0.22` are the **hue factor**, `1.9` is the **saturation factor**, respectively, to control the hue and saturation in the color similarity matching. 
  + Increasing the value of the hue factor makes the boundaries between the different colors more clear.
  + Increasing the value of the saturation factor increases the distance between the different saturation regions in the same color. 
+ $D_{ij}$ ss the color distance between the i and j points.

## <a name = "ranking"></a>Ranking

Using sort algorithm to simply rank the picture in the data set, return the first 10 results.

## <a name = "conclusion"></a>Conclusion

The above definition of color distance have intuitional alignment with human eye when regonizing different colors. And the different color factor and the saturation factor can be selected according to the different picture properties. It also revolved the problem that black and white have differential distance to other colors in the HSV Space.

**However, the number of pictures in the picture library is really small, the selection of the hue factor and the saturation factor in the definition of the color distance is limited, and the robustness of the system is not enough.**

#<a name = "user-interface-design"></a>User Interface Design

Used C# to develop a winform application to provide user responsive interface.

![](https://ws1.sinaimg.cn/large/006tNc79gy1fk2djpgpdej31k40xlagz.jpg)

![](https://ws1.sinaimg.cn/large/006tNc79gy1fk2djszkswj31kw0xz7c9.jpg)

![](https://ws2.sinaimg.cn/large/006tNc79gy1fk2djwhdn8j31kw0y1tg2.jpg)

![](https://ws1.sinaimg.cn/large/006tNc79gy1fk2djzhedhj31kw0xz45v.jpg)

![](https://ws2.sinaimg.cn/large/006tNc79gy1fk2dk2iwkfj31kw0xy7bw.jpg)
