namespace IOBox.SqlServer.Demo;

internal record NewMemberInsertDto(string Email);

internal record NewMemberWelcomeMessage(
    string MessageId,
    int MemberId,
    string MemberEmail);

internal record NewMemberBonusMessage(
    string MessageId,
    int MemberId,
    string MemberEmail);
