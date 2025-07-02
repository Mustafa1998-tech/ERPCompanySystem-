<?php

/**
 * تحقق من صحة المدخلات النصية
 * @param string $input المدخل
 * @return string المدخل بعد التنظيف
 */
function sanitize_input($input)
{
    if (is_array($input)) {
        return array_map('sanitize_input', $input);
    }
    return htmlspecialchars(strip_tags(trim($input)));
}

/**
 * تحقق من صحة البريد الإلكتروني
 * @param string $email البريد الإلكتروني
 * @return bool صحيح إذا كان البريد الإلكتروني صالحًا
 */
function is_valid_email($email)
{
    return filter_var($email, FILTER_VALIDATE_EMAIL);
}

/**
 * تحقق من صحة كلمة المرور
 * @param string $password كلمة المرور
 * @return bool صحيح إذا كانت كلمة المرور صالحة
 */
function is_valid_password($password)
{
    return strlen($password) >= 8;
}

/**
 * تحقق من صحة رقم الهاتف
 * @param string $phone رقم الهاتف
 * @return bool صحيح إذا كان رقم الهاتف صالحًا
 */
function is_valid_phone($phone)
{
    return preg_match('/^\+?\d{10,15}$/', $phone);
}

/**
 * تحقق من صحة رقم الهوية
 * @param string $id رقم الهوية
 * @return bool صحيح إذا كان رقم الهوية صالحًا
 */
function is_valid_id($id)
{
    return preg_match('/^\d{10}$/', $id);
}

/**
 * تحقق من صحة رقم البطاقة
 * @param string $card_number رقم البطاقة
 * @return bool صحيح إذا كان رقم البطاقة صالحًا
 */
function is_valid_card_number($card_number)
{
    return preg_match('/^\d{16}$/', $card_number);
}

/**
 * تحقق من صحة رقم التحقق
 * @param string $otp رقم التحقق
 * @return bool صحيح إذا كان رقم التحقق صالحًا
 */
function is_valid_otp($otp)
{
    return preg_match('/^\d{6}$/', $otp);
}

/**
 * توليد رقم تحقق عشوائي
 * @return string رقم التحقق
 */
function generate_otp()
{
    return str_pad(mt_rand(0, 999999), 6, '0', STR_PAD_LEFT);
}

/**
 * توليد رمز تفعيل عشوائي
 * @param int $length طول الرمز
 * @return string رمز التفعيل
 */
function generate_activation_code($length = 32)
{
    return bin2hex(random_bytes($length / 2));
}

/**
 * توليد رمز استعادة كلمة المرور
 * @return string رمز الاستعادة
 */
function generate_reset_token()
{
    return bin2hex(random_bytes(32));
}

/**
 * تحقق من صلاحية رمز الاستعادة
 * @param string $token رمز الاستعادة
 * @return bool صحيح إذا كان الرمز صالحًا
 */
function is_valid_reset_token($token)
{
    return strlen($token) === 64 && ctype_xdigit($token);
}

/**
 * توليد رمز جلسة عشوائي
 * @return string رمز الجلسة
 */
function generate_session_token()
{
    return bin2hex(random_bytes(32));
}

/**
 * تحقق من صلاحية رمز الجلسة
 * @param string $token رمز الجلسة
 * @return bool صحيح إذا كان الرمز صالحًا
 */
function is_valid_session_token($token)
{
    return strlen($token) === 64 && ctype_xdigit($token);
}

/**
 * تنظيف المدخلات من أكواد HTML خطرة
 * @param string $input المدخل
 * @return string المدخل بعد التنظيف
 */
function clean_html($input)
{
    $allowed = '<p><a><br><strong><em><i><b><u><ol><ul><li><h1><h2><h3><h4><h5><h6>';
    return strip_tags($input, $allowed);
}

/**
 * تشفير كلمة المرور
 * @param string $password كلمة المرور
 * @return string كلمة المرور المشفرة
 */
function hash_password($password)
{
    return password_hash($password, PASSWORD_DEFAULT);
}

/**
 * التحقق من صحة كلمة المرور
 * @param string $password كلمة المرور
 * @param string $hash كلمة المرور المشفرة
 * @return bool صحيح إذا كانت كلمة المرور متطابقة
 */
function verify_password($password, $hash)
{
    return password_verify($password, $hash);
}

/**
 * توليد رمز API عشوائي
 * @return string رمز API
 */
function generate_api_key()
{
    return bin2hex(random_bytes(32));
}

/**
 * تحقق من صلاحية رمز API
 * @param string $key رمز API
 * @return bool صحيح إذا كان الرمز صالحًا
 */
function is_valid_api_key($key)
{
    return strlen($key) === 64 && ctype_xdigit($key);
}

/**
 * توليد رمز تعريف عشوائي
 * @param int $length طول الرمز
 * @return string رمز التعريف
 */
function generate_reference()
{
    return date('Ymd') . '-' . bin2hex(random_bytes(4));
}

/**
 * تحقق من صحة رمز التعريف
 * @param string $reference رمز التعريف
 * @return bool صحيح إذا كان الرمز صالحًا
 */
function is_valid_reference($reference)
{
    return preg_match('/^\d{8}-[a-f0-9]{8}$/', $reference);
}

/**
 * تنظيف رقم الهاتف
 * @param string $phone رقم الهاتف
 * @return string رقم الهاتف بعد التنظيف
 */
function clean_phone($phone)
{
    return preg_replace('/[^0-9+]/', '', $phone);
}

/**
 * تنظيف رقم الهوية
 * @param string $id رقم الهوية
 * @return string رقم الهوية بعد التنظيف
 */
function clean_id($id)
{
    return preg_replace('/[^0-9]/', '', $id);
}

/**
 * تنظيف رقم البطاقة
 * @param string $card_number رقم البطاقة
 * @return string رقم البطاقة بعد التنظيف
 */
function clean_card_number($card_number)
{
    return preg_replace('/[^0-9]/', '', $card_number);
}

/**
 * تنظيف رمز التحقق
 * @param string $otp رمز التحقق
 * @return string رمز التحقق بعد التنظيف
 */
function clean_otp($otp)
{
    return preg_replace('/[^0-9]/', '', $otp);
}

/**
 * تنظيف رمز الجلسة
 * @param string $token رمز الجلسة
 * @return string رمز الجلسة بعد التنظيف
 */
function clean_session_token($token)
{
    return preg_replace('/[^a-f0-9]/i', '', $token);
}

/**
 * تنظيف رمز الاستعادة
 * @param string $token رمز الاستعادة
 * @return string رمز الاستعادة بعد التنظيف
 */
function clean_reset_token($token)
{
    return preg_replace('/[^a-f0-9]/i', '', $token);
}

/**
 * تنظيف رمز API
 * @param string $key رمز API
 * @return string رمز API بعد التنظيف
 */
function clean_api_key($key)
{
    return preg_replace('/[^a-f0-9]/i', '', $key);
}

/**
 * تنظيف رمز التعريف
 * @param string $reference رمز التعريف
 * @return string رمز التعريف بعد التنظيف
 */
function clean_reference($reference)
{
    return preg_replace('/[^0-9a-f-]/i', '', $reference);
}

/**
 * تنظيف رقم الهاتف مع التحقق من صحة التنسيق
 * @param string $phone رقم الهاتف
 * @return string رقم الهاتف بعد التنظيف والتحقق
 */
function clean_and_validate_phone($phone)
{
    $cleaned = clean_phone($phone);
    return is_valid_phone($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رقم الهوية مع التحقق من صحة التنسيق
 * @param string $id رقم الهوية
 * @return string رقم الهوية بعد التنظيف والتحقق
 */
function clean_and_validate_id($id)
{
    $cleaned = clean_id($id);
    return is_valid_id($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رقم البطاقة مع التحقق من صحة التنسيق
 * @param string $card_number رقم البطاقة
 * @return string رقم البطاقة بعد التنظيف والتحقق
 */
function clean_and_validate_card_number($card_number)
{
    $cleaned = clean_card_number($card_number);
    return is_valid_card_number($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رمز التحقق مع التحقق من صحة التنسيق
 * @param string $otp رمز التحقق
 * @return string رمز التحقق بعد التنظيف والتحقق
 */
function clean_and_validate_otp($otp)
{
    $cleaned = clean_otp($otp);
    return is_valid_otp($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رمز الجلسة مع التحقق من صحة التنسيق
 * @param string $token رمز الجلسة
 * @return string رمز الجلسة بعد التنظيف والتحقق
 */
function clean_and_validate_session_token($token)
{
    $cleaned = clean_session_token($token);
    return is_valid_session_token($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رمز الاستعادة مع التحقق من صحة التنسيق
 * @param string $token رمز الاستعادة
 * @return string رمز الاستعادة بعد التنظيف والتحقق
 */
function clean_and_validate_reset_token($token)
{
    $cleaned = clean_reset_token($token);
    return is_valid_reset_token($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رمز API مع التحقق من صحة التنسيق
 * @param string $key رمز API
 * @return string رمز API بعد التنظيف والتحقق
 */
function clean_and_validate_api_key($key)
{
    $cleaned = clean_api_key($key);
    return is_valid_api_key($cleaned) ? $cleaned : false;
}

/**
 * تنظيف رمز التعريف مع التحقق من صحة التنسيق
 * @param string $reference رمز التعريف
 * @return string رمز التعريف بعد التنظيف والتحقق
 */
function clean_and_validate_reference($reference)
{
    $cleaned = clean_reference($reference);
    return is_valid_reference($cleaned) ? $cleaned : false;
}

/**
 * تنظيف البريد الإلكتروني
 * @param string $email البريد الإلكتروني
 * @return string البريد الإلكتروني بعد التنظيف
 */
function clean_email($email)
{
    return filter_var($email, FILTER_SANITIZE_EMAIL);
}

/**
 * تنظيف البريد الإلكتروني مع التحقق من صحة التنسيق
 * @param string $email البريد الإلكتروني
 * @return string البريد الإلكتروني بعد التنظيف والتحقق
 */
function clean_and_validate_email($email)
{
    $cleaned = clean_email($email);
    return is_valid_email($cleaned) ? $cleaned : false;
}
