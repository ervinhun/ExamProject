export default function Privacy() {
    return (
        <div className="container mx-auto px-4 py-8">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-4xl font-bold text-primary mb-3">
                        Privacy Policy
                    </h1>
                    <p className="text-xl text-base-content/70">
                        How Jerne Idrætsforening processes and protects your personal data
                    </p>
                </div>

                {/* Introduction */}
                <div className="prose prose-lg max-w-none mb-8">
                    <p className="text-base-content/80">
                        Jerne Idrætsforening (“Jerne IF”, “we”, “us”) respects your privacy
                        and is committed to protecting your personal data. This Privacy
                        Policy explains how we collect, store, and use personal data when
                        you use our website and internal systems.
                    </p>
                </div>

                {/* Data Controller */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Data Controller</h2>
                        <div className="text-base-content/80 space-y-1">
                            <p><strong>Organization:</strong> Jerne Idrætsforening</p>
                            <p><strong>Website:</strong> https://jerneif.dk</p>
                            <p>
                                All personal data is handled internally by Jerne IF.
                                For questions regarding data protection, please contact
                                the board of the association.
                            </p>
                        </div>
                    </div>
                </div>

                {/* Data We Collect */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">What Personal Data We Collect</h2>
                    <div className="space-y-3 text-base-content/80">
                        <p>
                            We only collect personal data that is necessary for operating
                            our services and managing memberships. This may include:
                        </p>
                        <ul className="list-disc pl-6 space-y-1">
                            <li>Name</li>
                            <li>Email address</li>
                            <li>Membership status and related information</li>
                            <li>Internal transaction or reference numbers</li>
                            <li>Technical information such as login timestamps and IP address</li>
                        </ul>
                    </div>
                </div>

                {/* Purpose */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Purpose of Data Processing</h2>
                        <div className="space-y-2 text-base-content/80">
                            <p>Your personal data is processed solely for internal purposes, including:</p>
                            <ul className="list-disc pl-6 space-y-1">
                                <li>Administration of memberships and user accounts</li>
                                <li>Operation of internal systems and tools</li>
                                <li>Internal communication related to club activities</li>
                                <li>Security, logging, and prevention of misuse</li>
                                <li>Compliance with legal and accounting requirements</li>
                            </ul>
                        </div>
                    </div>
                </div>

                {/* No Third Parties */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">No Sharing With Third Parties</h2>
                    <p className="text-base-content/80">
                        Jerne Idrætsforening does <strong>not</strong> sell, rent, or share
                        personal data with third parties. All personal data is used
                        exclusively within the organization and is only accessible to
                        authorized individuals who require access for administrative
                        purposes.
                    </p>
                </div>

                {/* Soft Delete */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Data Retention & Soft Deletion</h2>
                        <p className="text-base-content/80">
                            When a user account or related data is removed, Jerne IF applies
                            a <strong>soft deletion</strong> process. This means that the data
                            is no longer actively used and is hidden from normal access,
                            but may be retained internally for documentation, auditing,
                            or legal purposes.
                        </p>
                        <p className="text-base-content/80 mt-3">
                            Soft-deleted data is protected in the same way as active data
                            and is only accessible to authorized personnel.
                        </p>
                    </div>
                </div>

                {/* Security */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Data Security</h2>
                    <p className="text-base-content/80">
                        We apply appropriate technical and organizational security measures
                        to protect personal data against unauthorized access, alteration,
                        loss, or misuse. Access to personal data is restricted to persons
                        with a legitimate internal need.
                    </p>
                </div>

                {/* Rights */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Your Rights</h2>
                        <div className="space-y-2 text-base-content/80">
                            <p>You have the right to:</p>
                            <ul className="list-disc pl-6 space-y-1">
                                <li>Request access to your personal data</li>
                                <li>Request correction of incorrect or outdated data</li>
                                <li>Request deletion or deactivation of your account</li>
                                <li>Object to or restrict processing of your data</li>
                                <li>Withdraw consent where applicable</li>
                            </ul>
                            <p>
                                You also have the right to file a complaint with the
                                Danish Data Protection Agency (Datatilsynet).
                            </p>
                        </div>
                    </div>
                </div>

                {/* Closing */}
                <div className="text-center py-6">
                    <p className="text-base-content/70">
                        This Privacy Policy may be updated to reflect changes in our
                        internal processes or legal requirements.
                    </p>
                    <p className="text-lg font-semibold text-primary mt-3">
                        Jerne Idrætsforening — your data stays with us
                    </p>
                </div>
            </div>
        </div>
    );
}
