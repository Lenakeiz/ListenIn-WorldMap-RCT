<?php
$id_user = $_POST['id_user'];
$file_name = $_POST['file_name'];
$folder_name = $_POST['folder_name'];

$root_path = '../../files/'.$id_user.'/'.$folder_name.'/';
$file_path = $root_path.$file_name;

if (file_exists($file_path))
{
	echo file_get_contents($file_path);
}
else
{
	$file_path = $root_path.'CORE*';
	if(count(glob($file_path)) != 0)
	{
		echo file_get_contents(glob($file_path)[0]);
	}
	else
	{
		echo "error";
	}
}
?>
