   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � Title$  Чат клиент��5   	   $	��450��6   $	��800��6)    � 	Resources 5
      #   Client.SystemMessageConverter   SystemMessageConverter'   !Client.MessageBackgroundConverter  MessageBackgroundConverter��%:   &	        &	        5   
   6'     5   
   6+   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��5q�6;   .+�5      �� -   5         ��Name$ UsernameTextBox��5      $	��150��6/   $��5q�6;   $��
Гость��5       � ToolTip$$ Введите ваше имя��6#   �� 5      -   5        $ ServerIpTextBox��5      $	��100��6/   $��5q�6;   $��172.22.1.89��5      $ IP сервера��6)   �� 5      -   5        $ ServerPortTextBox��5      $��50��61   $��5q�6<   $��11000��5      $ Порт сервера��6#   �� 5      -   5         ��Name$ ConnectButton��5      $��Подключиться��6,   $��5q�6C   $	��100��5       6!   �� 5!      -   5        $ DisconnectButton��5!      $��Отключиться��6/   $��5q�6E   $	��100��5"      {�.  6@   5#      �� 5&   
   -   5         ��Name$ ChatMessagesListView��5&      $��1��61   $��5q�6>   ��5'      �� 5(      .R�5)      �� $��5q�6!   .+�5*      �� $
��Bold"�6$   ��5+   $   �� *Sender+��5,   $   �� *
IsSystem+ �� 	Converter# [  �� 5-      	 �� Padding$	 5q�5/   $   ��5-   $   �� *Text+��5.   $   �� *IsOwn+# [ 5/   $   �� 50      $��10(�6$   
���������51   $   �� *	Timestamp+
 �� StringFormat$
 {0:HH:mm:ss}��52      53      55      � 58   
   $��2��6   $��5q�6    � ColumnDefinitions 59      �� 5:      $��* �6#   �� 5;      $
��Auto �6#   .+�5=      �� -   5        $ MessageTextBox��5=      $��0��6.   $��5q�6>   5>      �� 5?      -	   5        $ 
SendButton��5?      $��1��6)   $��Отправить��69   $��5q�6M   $	��100��5@      {�.  6:   5A      �� 5D   
   $��3��6   .@�5E      �� .��5F      A� -
   5         A�Name$ ConnectionIndicator��5F      $��10��67   $��10��6B   
���  ��6N   5G      �� 5H      ��6   �� *ConnectionStatus+5I      5J      5K      