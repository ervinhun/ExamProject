export default function getAge(dateString) {
    const today = new Date();
    const birth = new Date(dateString);
    console.log("Date Of birth ", dateString)
    let age = today.getFullYear() - birth.getFullYear();
    const hasHadBirthdayThisYear =
        today.getMonth() > birth.getMonth() ||
        (today.getMonth() === birth.getMonth() &&
            today.getDate() >= birth.getDate());

    if (!hasHadBirthdayThisYear) {
        age--;
    }

    return age;
}