<?php
$id_user = $_POST['id_user'];
$file_name = $_POST['file_name'];
$folder_name = $_POST['folder_name'];

$file_path = '../../files/'.$id_user.'/'.$folder_name.'/'.$file_name;

if (file_exists($file_path))
{
	echo file_get_contents($file_path);
}
else
{
	echo "error";
}

?>
