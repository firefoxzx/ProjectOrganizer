   M S B A M L   `   `   `  ����K  GProjectOrganizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ProjectOrganizerProjectOrganizer  5   	   !    ProjectOrganizer.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:ProjectOrganizer     5   	     � Title$  Project Organizer��5   	    � 	Resources 5
   
      ShowSidebar �� 
TargetName �� TargetProperty R� KeyTime R� Value  HideSidebar��%�  &	        &	 �     �� 5      .��5      T� $ SidebarColumn��60   $ Width�5   0   ..�5      i� $ 0:0:0��6-   $ 0��6=   i� 5      $ 0:0:0.5��6-   $	 300��6?   5      5      �� 5      .��5      T� $ SidebarColumn��60   $ Width�5   0   ..�5      i� $ 0:0:0��6-   $	 300��6=   i� 5      $ 0:0:0.5��6-   $ 0��6?   5      5      5      .��5      �  � ColumnDefinitions 5   
   �� 5       $��* �6   �� 5!      -   5         ��Name$ SidebarColumn��5!      $��0 �66   .+�5'   
   � $��0��6    � RowDefinitions 5(      �� 5)      $��30 �6    �� 5*      $��50 �6    �� 5+      $��* �6    .+�58      �� $��0��6   	 �� Orientation$	 
Horizontal=�6&   $��Right=�6?   .+�5:      �� $��Settings��6   
�������$��10(�6A   $
��Bold"�6O   $��5,0,5,0q�6a   �� 5;      -   5        $��Scheduled Tasks��5;      
�������$��10(�6H   $��5,0,5,0q�6V   6g   5<      �� 5>      -   5        
 ��Name$
 scrollViewer��5>      $��1��61   $��Hidden=�6>   $��Disabled=�5?      $
��Auto��68   $
��Auto��6E   $
��Left=�5@      $��Center=�6,   .��5B      �� -   5         ��Name$ ProjectTypesPanel��5B      $	 
Horizontal=�68   $��50,0,0,0q�6Q   $��Center=�6c   5D      5F      �� 5G      -   5        $��1��5G      $	��←��6&   $��30��62   $��30��6=   $
��Left=�6I   $��Center=�5H      $��
10,10,10,8q�6G   
�������$��0q�5I   &   $��18(�6:   �� 5K      -   5        $��1��5K      $	��→��6&   $��30��62   $��30��6=   $��Right=�6I   $��Center=�5L      $��
10,10,10,8q�6H   
�������$��0q�5M   &   $��18(�6:   � 5P      $��2��6   .+�5R      �� $
��Auto=�6    .��5S      � -   5         �Name$ ProjectsPanel��5S        � Orientation$ 
Horizontal=�67   5T      �� 5W      -   5        $��CreateProjectBtn��5W      $��60��5X      $��60��6$   $	��➕��5Y      
������
�������$��30(�5\      $
��Bold"�5]      $��Right=�5^      $��Bottom=�5_      $��20q�5`      5a      5b      5d      ]� 5f   
   $��1��6   $	��300��6$   ���#2C3E50.+�5g      �� $
��Auto=�6   .��5h      �� .+�5i      �� $��Scheduled Tasks��6    $��16(�67   $��DemiBold"�6E   
�������6[   �� 5l      $��Today��6    $��14(�6-   $
��Bold"�6;   
�������$��5q�6`   �� 5m      ��6#   �� *
TodayTasks+��5n      �� 5o      .R�5p   "   �� 
�������$��10,0,0,5q�6Y   ��6,   �� *Name+6Y   5q      5s      �� 5v      ��6#   �� *GroupedTasks+��5w      �� 5x      .R�5y   "   �� .+�5z   &   �� $��14(�5{   0   $
��Bold"�6>   
�������$��5q�6c   ��5z   0   �� *Key+ �� StringFormat$ {0:MMMM dd, yyyy}��5{   c   �� 5|   &   ��63   �� *Value+��5}   *   �� 5~   .   .R�5   2   �� 
�������$��10,0,0,5q�6i   ��6<   �� *Name+6i   5�   /   5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      