<?php 
class dbConnection
{
   private static $instance = null;
   public static function get()
   {
       if(self::$instance == null)
       {
           try
           {
				self::$instance = new PDO('mysql:host=localhost;dbname=softrsav_li_game', 'root', 'ievh893u90',
				[ PDO::ATTR_EMULATE_PREPARES => true]);
				self::$instance->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
				self::$instance->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
				self::$instance->setAttribute(PDO::MYSQL_ATTR_MAX_BUFFER_SIZE, 1024*1024*15);  // increase buffer size from the default 1M to 5M
           } 
           catch(PDOException $e)
           {
               // Handle this properly
               throw $e;
           }
       }
       return self::$instance;
   }
}
// Set timezone
 date_default_timezone_set("Europe/London");
?>
